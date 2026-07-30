import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getMainOrganizations, getSubOrganizations, getEmployeesByOrganization } from '../api/employeeApi';
import './EmployeeList.css';

function EmployeeList() {
  const [mainOrgs, setMainOrgs] = useState([]);
  const [activeTab, setActiveTab] = useState(null);
  const [subOrgs, setSubOrgs] = useState([]);
  const [selectedSubOrg, setSelectedSubOrg] = useState(null);
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [loadingSubOrgs, setLoadingSubOrgs] = useState(false);
  const [loadingEmployees, setLoadingEmployees] = useState(false);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [showSalaryModal, setShowSalaryModal] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const navigate = useNavigate();

  const user = JSON.parse(localStorage.getItem('user') || '{}');

  useEffect(() => {
    if (!localStorage.getItem('user')) {
      navigate('/login');
      return;
    }
    fetchMainOrganizations();
  }, [navigate]);

  const fetchMainOrganizations = async () => {
    try {
      setLoading(true);
      const data = await getMainOrganizations();
      setMainOrgs(data);
      if (data.length > 0) {
        setActiveTab(data[0].id);
        loadSubOrganizations(data[0].id);
      }
    } catch (err) {
      setError('Không thể tải dữ liệu tổ chức');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const loadSubOrganizations = async (parentId) => {
    try {
      setLoadingSubOrgs(true);
      setSubOrgs([]);
      setEmployees([]);
      setSelectedSubOrg(null);
      const data = await getSubOrganizations(parentId);
      setSubOrgs(data);
    } catch (err) {
      setError('Không thể tải phòng ban/cụm/tổ');
      console.error(err);
    } finally {
      setLoadingSubOrgs(false);
    }
  };

  const loadEmployees = async (orgId, orgName) => {
    try {
      setLoadingEmployees(true);
      setSelectedSubOrg({ id: orgId, name: orgName });
      const data = await getEmployeesByOrganization(orgId);
      setEmployees(data);
    } catch (err) {
      setError('Không thể tải danh sách nhân viên');
      console.error(err);
    } finally {
      setLoadingEmployees(false);
    }
  };

  const handleTabChange = (orgId) => {
    setActiveTab(orgId);
    loadSubOrganizations(orgId);
  };

  const handleLogout = () => {
    localStorage.removeItem('user');
    navigate('/login');
  };

  const formatDate = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
  };

  const formatMonthYear = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return `${date.getMonth() + 1}/${date.getFullYear()}`;
  };

  const filteredEmployees = employees.filter(emp =>
    emp.fullName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    emp.socialInsurance?.includes(searchTerm) ||
    emp.identityCardNumber?.includes(searchTerm)
  );

  // Sort by total salary (highest to lowest)
  const sortedEmployees = [...filteredEmployees].sort((a, b) => {
    return (b.totalSalary || 0) - (a.totalSalary || 0);
  });

  const getOrgTypeName = (type) => {
    switch (type) {
      case 2: return 'Phòng';
      case 3: return 'Cụm';
      case 4: return 'Tổ';
      default: return '';
    }
  };

  const handleShowSalaryDetails = (employee) => {
    setSelectedEmployee(employee);
    setShowSalaryModal(true);
  };

  const handleCloseSalaryModal = () => {
    setShowSalaryModal(false);
    setSelectedEmployee(null);
  };

  const formatCurrency = (amount) => {
    if (!amount) return '0';
    return new Intl.NumberFormat('vi-VN').format(amount);
  };

  const getAllowanceLabel = (employee) => {
    if (!employee.allowanceName) return 'Không có';
    
    // Determine if it's PCTN (Responsibility) or PCCV (Job)
    if (employee.allowanceName.includes('trách nhiệm')) {
      return `PCTN: ${employee.allowanceCoefficient || 0}`;
    } else if (employee.allowanceName.includes('công việc')) {
      return `PCCV: ${employee.allowanceCoefficient || 0}`;
    }
    return `${employee.allowanceCoefficient || 0}`;
  };

  if (loading) {
    return <div className="loading">Đang tải...</div>;
  }

  return (
    <div className="employee-list-container">
      <header className="header">
        <div className="header-left">
          <h1>MyHR - Quản lý nhân sự</h1>
        </div>
        <div className="header-right">
          <span className="user-name">Xin chào, {user.fullName}</span>
          <button onClick={handleLogout} className="logout-btn">Đăng xuất</button>
        </div>
      </header>

      <main className="main-content">
        <div className="toolbar">
          <h2>Danh sách nhân viên</h2>
          <button onClick={() => navigate('/employees/add')} className="btn-add-employee">
            Thêm nhân viên
          </button>
        </div>

        {error && <div className="error-message">{error}</div>}

        {/* Main Organization Tabs */}
        <div className="tabs-container">
          {mainOrgs.map(org => (
            <button
              key={org.id}
              className={`tab-button ${activeTab === org.id ? 'active' : ''}`}
              onClick={() => handleTabChange(org.id)}
            >
              {org.name}
            </button>
          ))}
        </div>

        {/* Employees Table */}
        <div className="employees-section">
          <div className="employees-header">
            <div className="sub-org-selector">
              <label htmlFor="subOrg">Phòng ban/Cụm/Tổ:</label>
              {loadingSubOrgs ? (
                <div className="loading-select">Đang tải...</div>
              ) : (
                <select
                  id="subOrg"
                  value={selectedSubOrg?.id || ''}
                  onChange={(e) => {
                    const selected = subOrgs.find(s => s.id === e.target.value);
                    if (selected) {
                      loadEmployees(selected.id, selected.name);
                    }
                  }}
                  className="sub-org-select"
                  disabled={subOrgs.length === 0}
                >
                  <option value="">-- Chọn đơn vị --</option>
                  {subOrgs.map(subOrg => (
                    <option key={subOrg.id} value={subOrg.id}>
                      {getOrgTypeName(subOrg.type)} - {subOrg.name}
                    </option>
                  ))}
                </select>
              )}
            </div>
            <input
              type="text"
              placeholder="Tìm kiếm theo tên, mã BHXH, CCCD..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="search-input"
            />
          </div>

          {loadingEmployees ? (
            <div className="loading-employees">Đang tải nhân viên...</div>
          ) : !selectedSubOrg ? (
            <div className="no-selection">Vui lòng chọn phòng ban/cụm/tổ để xem danh sách nhân viên</div>
          ) : filteredEmployees.length === 0 ? (
            <div className="no-data">Không có nhân viên trong đơn vị này</div>
          ) : (
            <div className="table-wrapper">
              <table className="employee-table">
                <thead>
                  <tr>
                    <th>STT</th>
                    <th>Họ và tên</th>
                    <th title="Giới tính">GT</th>
                    <th title="Mã số BHXH">Mã BHXH</th>
                    <th>Ngày sinh</th>
                    <th title="Căn cước công dân">CCCD</th>
                    <th title="Thời gian đóng BHXH">TG BHXH</th>
                    <th title="Chuyên môn nghiệp vụ">Chuyên môn</th>
                    <th>Trình độ</th>
                    <th title="Bậc lương">Bậc</th>
                    <th>Tổng lương</th>
                  </tr>
                </thead>
                <tbody>
                  {sortedEmployees.map((emp, index) => (
                    <tr key={emp.id}>
                      <td className="stt-cell">{index + 1}</td>
                      <td className="name-cell">{emp.fullName}</td>
                      <td className="gender-cell">{emp.sex === 0 ? 'Nữ' : 'Nam'}</td>
                      <td>{emp.socialInsurance}</td>
                      <td>{formatDate(emp.dob)}</td>
                      <td>{emp.identityCardNumber}</td>
                      <td>{formatMonthYear(emp.socialInsuranceContributionDate)}</td>
                      <td className="position-cell">{emp.positionName}</td>
                      <td className="profession-cell">{emp.professionName}</td>
                      <td className="grade-cell">{emp.currentSalaryGrade}</td>
                      <td 
                        className="salary-cell clickable"
                        onClick={() => handleShowSalaryDetails(emp)}
                        title="Nhấn để xem chi tiết lương"
                      >
                        <span className="salary-amount">{formatCurrency(emp.totalSalary)}</span>
                        <span className="salary-hint">VNĐ</span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <div className="employee-count">
                Tổng số: {filteredEmployees.length} nhân viên
              </div>
            </div>
          )}
        </div>
      </main>

      {/* Salary Details Modal */}
      {showSalaryModal && selectedEmployee && (
        <div className="modal-overlay" onClick={handleCloseSalaryModal}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Chi tiết lương - {selectedEmployee.fullName}</h3>
              <button className="modal-close-btn" onClick={handleCloseSalaryModal}>×</button>
            </div>
            <div className="modal-body">
              <div className="salary-detail-row">
                <span className="label">Lương cơ bản:</span>
                <span className="value">{formatCurrency(2340000)} VNĐ</span>
              </div>
              <div className="salary-detail-row">
                <span className="label">Hệ số lương:</span>
                <span className="value">{selectedEmployee.salaryCof || 0}</span>
              </div>
              <div className="salary-detail-row">
                <span className="label">Phụ cấp:</span>
                <span className="value">{getAllowanceLabel(selectedEmployee)}</span>
              </div>
              <div className="salary-detail-divider"></div>
              <div className="salary-detail-row total">
                <span className="label">Tổng lương:</span>
                <span className="value highlight">{formatCurrency(selectedEmployee.totalSalary)} VNĐ</span>
              </div>
              <div className="salary-formula">
                <small>Công thức: Lương cơ bản × (Hệ số lương + Phụ cấp)</small>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default EmployeeList;
