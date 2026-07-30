import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  addEmployee,
  getAllPositions,
  getAllProfessions,
  getAllAllowances,
  getMainOrganizations,
  getSubOrganizations
} from '../api/employeeApi';
import './AddEmployee.css';

function AddEmployee() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // Reference data
  const [mainOrgs, setMainOrgs] = useState([]);
  const [subOrgs, setSubOrgs] = useState([]);
  const [positions, setPositions] = useState([]);
  const [professions, setProfessions] = useState([]);
  const [allowances, setAllowances] = useState([]);

  // Form data
  const [formData, setFormData] = useState({
    fullName: '',
    sex: 0, // 0 = Male, 1 = Female
    socialInsurance: '',
    dob: '',
    identityCardNumber: '',
    socialInsuranceContributionDate: '',
    organizationId: '',
    positionId: '',
    professionId: '',
    allowanceId: '',
    currentGradeLevel: 1,
    salaryEffectiveFrom: new Date().toISOString().split('T')[0],
    salaryReason: 'Tuyển dụng mới',
    fixedSalaryAmount: null
  });

  const [selectedMainOrg, setSelectedMainOrg] = useState('');

  useEffect(() => {
    if (!localStorage.getItem('user')) {
      navigate('/login');
      return;
    }
    loadReferenceData();
  }, [navigate]);

  const loadReferenceData = async () => {
    try {
      setLoading(true);
      const [mainOrgsData, positionsData, professionsData, allowancesData] = await Promise.all([
        getMainOrganizations(),
        getAllPositions(),
        getAllProfessions(),
        getAllAllowances()
      ]);
      
      setMainOrgs(mainOrgsData);
      setPositions(positionsData);
      setProfessions(professionsData);
      setAllowances(allowancesData);
    } catch (err) {
      setError('Không thể tải dữ liệu tham chiếu');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleMainOrgChange = async (mainOrgId) => {
    setSelectedMainOrg(mainOrgId);
    setFormData({ ...formData, organizationId: '' });
    setSubOrgs([]);
    
    if (mainOrgId) {
      try {
        const subOrgsData = await getSubOrganizations(mainOrgId);
        setSubOrgs(subOrgsData);
      } catch (err) {
        console.error('Error loading sub organizations:', err);
      }
    }
  };

  const handleInputChange = (e) => {
    const { name, value, type } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'number' ? (value ? parseInt(value) : '') : value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    
    // Validation
    if (!formData.fullName || !formData.identityCardNumber || !formData.socialInsurance) {
      setError('Vui lòng điền đầy đủ thông tin bắt buộc');
      return;
    }

    if (!formData.organizationId || !formData.positionId || !formData.professionId) {
      setError('Vui lòng chọn tổ chức, chức vụ và nghiệp vụ chuyên môn');
      return;
    }

    try {
      setLoading(true);
      
      // Prepare data for API
      const apiData = {
        ...formData,
        sex: parseInt(formData.sex),
        currentGradeLevel: parseInt(formData.currentGradeLevel) || 1,
        allowanceId: formData.allowanceId || null,
        fixedSalaryAmount: formData.fixedSalaryAmount ? parseFloat(formData.fixedSalaryAmount) : null
      };

      await addEmployee(apiData);
      setSuccess('Thêm nhân viên thành công!');
      
      // Reset form
      setTimeout(() => {
        navigate('/employees');
      }, 2000);
      
    } catch (err) {
      setError(err.response?.data?.message || 'Thêm nhân viên thất bại. Vui lòng kiểm tra lại thông tin.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    navigate('/employees');
  };

  if (loading && mainOrgs.length === 0) {
    return <div className="loading">Đang tải...</div>;
  }

  return (
    <div className="add-employee-container">
      <div className="add-employee-header">
        <h1>Thêm Nhân Viên Mới</h1>
        <button onClick={handleCancel} className="btn-back">
          ← Quay lại
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}
      {success && <div className="success-message">{success}</div>}

      <form onSubmit={handleSubmit} className="add-employee-form">
        {/* Personal Information Section */}
        <div className="form-section">
          <h2>Thông tin cá nhân</h2>
          
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="fullName">Họ và tên <span className="required">*</span></label>
              <input
                type="text"
                id="fullName"
                name="fullName"
                value={formData.fullName}
                onChange={handleInputChange}
                placeholder="Nguyễn Văn A"
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="sex">Giới tính <span className="required">*</span></label>
              <select
                id="sex"
                name="sex"
                value={formData.sex}
                onChange={handleInputChange}
                required
              >
                <option value={0}>Nam</option>
                <option value={1}>Nữ</option>
              </select>
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="identityCardNumber">Số CCCD <span className="required">*</span></label>
              <input
                type="text"
                id="identityCardNumber"
                name="identityCardNumber"
                value={formData.identityCardNumber}
                onChange={handleInputChange}
                placeholder="001234567890"
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="dob">Ngày sinh <span className="required">*</span></label>
              <input
                type="date"
                id="dob"
                name="dob"
                value={formData.dob}
                onChange={handleInputChange}
                required
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="socialInsurance">Mã số BHXH <span className="required">*</span></label>
              <input
                type="text"
                id="socialInsurance"
                name="socialInsurance"
                value={formData.socialInsurance}
                onChange={handleInputChange}
                placeholder="1234567890"
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="socialInsuranceContributionDate">Ngày đóng BHXH <span className="required">*</span></label>
              <input
                type="date"
                id="socialInsuranceContributionDate"
                name="socialInsuranceContributionDate"
                value={formData.socialInsuranceContributionDate}
                onChange={handleInputChange}
                required
              />
            </div>
          </div>
        </div>

        {/* Organization & Position Section */}
        <div className="form-section">
          <h2>Thông tin công việc</h2>
          
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="mainOrg">Tổ chức chính <span className="required">*</span></label>
              <select
                id="mainOrg"
                value={selectedMainOrg}
                onChange={(e) => handleMainOrgChange(e.target.value)}
                required
              >
                <option value="">-- Chọn tổ chức chính --</option>
                {mainOrgs.map(org => (
                  <option key={org.id} value={org.id}>
                    {org.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="organizationId">Phòng ban/Cụm/Tổ <span className="required">*</span></label>
              <select
                id="organizationId"
                name="organizationId"
                value={formData.organizationId}
                onChange={handleInputChange}
                required
                disabled={!selectedMainOrg}
              >
                <option value="">-- Chọn phòng ban/cụm/tổ --</option>
                {subOrgs.map(org => (
                  <option key={org.id} value={org.id}>
                    {org.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="positionId">Chức vụ <span className="required">*</span></label>
              <select
                id="positionId"
                name="positionId"
                value={formData.positionId}
                onChange={handleInputChange}
                required
              >
                <option value="">-- Chọn chức vụ --</option>
                {positions.map(pos => (
                  <option key={pos.id} value={pos.id}>
                    {pos.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="professionId">Nghiệp vụ chuyên môn <span className="required">*</span></label>
              <select
                id="professionId"
                name="professionId"
                value={formData.professionId}
                onChange={handleInputChange}
                required
              >
                <option value="">-- Chọn nghiệp vụ chuyên môn --</option>
                {professions.map(prof => (
                  <option key={prof.id} value={prof.id}>
                    {prof.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="allowanceId">Phụ cấp</label>
              <select
                id="allowanceId"
                name="allowanceId"
                value={formData.allowanceId}
                onChange={handleInputChange}
              >
                <option value="">-- Không có phụ cấp --</option>
                {allowances.map(allow => (
                  <option key={allow.id} value={allow.id}>
                    {allow.name} (Hệ số: {allow.coefficient})
                  </option>
                ))}
              </select>
            </div>
          </div>
        </div>

        {/* Salary Section */}
        <div className="form-section">
          <h2>Thông tin lương</h2>
          
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="currentGradeLevel">Bậc lương hiện tại</label>
              <input
                type="number"
                id="currentGradeLevel"
                name="currentGradeLevel"
                value={formData.currentGradeLevel}
                onChange={handleInputChange}
                min="1"
                max="12"
              />
              <small>Để trống hoặc 0 nếu là lương cố định (Ban điều hành)</small>
            </div>

            <div className="form-group">
              <label htmlFor="salaryEffectiveFrom">Ngày hiệu lực <span className="required">*</span></label>
              <input
                type="date"
                id="salaryEffectiveFrom"
                name="salaryEffectiveFrom"
                value={formData.salaryEffectiveFrom}
                onChange={handleInputChange}
                required
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="fixedSalaryAmount">Lương cố định (VNĐ)</label>
              <input
                type="number"
                id="fixedSalaryAmount"
                name="fixedSalaryAmount"
                value={formData.fixedSalaryAmount || ''}
                onChange={handleInputChange}
                min="0"
                step="1000"
                placeholder="Chỉ dành cho Ban điều hành"
              />
              <small>Chỉ áp dụng cho nhân viên Ban điều hành</small>
            </div>

            <div className="form-group">
              <label htmlFor="salaryReason">Lý do</label>
              <input
                type="text"
                id="salaryReason"
                name="salaryReason"
                value={formData.salaryReason}
                onChange={handleInputChange}
                placeholder="Tuyển dụng mới"
              />
            </div>
          </div>
        </div>

        {/* Form Actions */}
        <div className="form-actions">
          <button type="button" onClick={handleCancel} className="btn-cancel" disabled={loading}>
            Hủy
          </button>
          <button type="submit" className="btn-submit" disabled={loading}>
            {loading ? 'Đang xử lý...' : 'Thêm nhân viên'}
          </button>
        </div>
      </form>
    </div>
  );
}

export default AddEmployee;
