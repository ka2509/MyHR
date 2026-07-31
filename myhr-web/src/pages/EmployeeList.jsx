import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getMainOrganizations, getSubOrganizations, getEmployeesByOrganization, deleteEmployee } from '../api/employeeApi';
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
  const [openMenuId, setOpenMenuId] = useState(null);
  const [menuPos, setMenuPos] = useState({ top: 0, right: 0 });
  const [showColumnFilter, setShowColumnFilter] = useState(false);
  const [visibleColumns, setVisibleColumns] = useState(() => {
    const saved = localStorage.getItem('visibleColumns');
    return saved ? JSON.parse(saved) : {
      stt: true,
      name: true,
      gender: true,
      socialInsurance: true,
      dob: true,
      identityCard: true,
      insuranceDate: true,
      position: true,
      profession: true,
      grade: true,
      salary: true,
    };
  });
  const navigate = useNavigate();

  useEffect(() => {
    if (!localStorage.getItem('user')) {
      navigate('/login');
      return;
    }
    fetchMainOrganizations();
  }, [navigate]);

  useEffect(() => {
    const handleClickOutside = (e) => {
      if (!e.target.closest('.btn-menu') && !e.target.closest('.action-dropdown')) {
        setOpenMenuId(null);
      }
      if (!e.target.closest('.column-filter-btn') && !e.target.closest('.column-filter-dropdown')) {
        setShowColumnFilter(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

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

  const handleMenuToggle = (e, empId) => {
    if (openMenuId === empId) {
      setOpenMenuId(null);
      return;
    }
    const rect = e.currentTarget.getBoundingClientRect();
    setMenuPos({
      top: rect.bottom + 4,
      right: window.innerWidth - rect.right,
    });
    setOpenMenuId(empId);
  };

  const handleDelete = async (emp) => {
    setOpenMenuId(null);
    if (!window.confirm(`Bạn có chắc muốn xoá nhân viên "${emp.fullName}" không?\nHành động này không thể hoàn tác.`)) return;
    try {
      await deleteEmployee(emp.id);
      setEmployees(prev => prev.filter(e => e.id !== emp.id));
    } catch (err) {
      alert('Xoá nhân viên thất bại. Vui lòng thử lại.');
      console.error(err);
    }
  };

  const formatCurrency = (amount) => {
    if (!amount) return '0';
    return new Intl.NumberFormat('vi-VN').format(amount);
  };

  const handleColumnToggle = (column) => {
    const updated = { ...visibleColumns, [column]: !visibleColumns[column] };
    setVisibleColumns(updated);
    localStorage.setItem('visibleColumns', JSON.stringify(updated));
  };

  const columnLabels = {
    stt: 'STT',
    name: 'Họ và tên',
    gender: 'Giới tính (GT)',
    socialInsurance: 'Mã BHXH',
    dob: 'Ngày sinh',
    identityCard: 'CCCD',
    insuranceDate: 'Thời gian BHXH',
    position: 'Chuyên môn',
    profession: 'Trình độ',
    grade: 'Bậc',
    salary: 'Tổng lương',
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
    <div className="employee-list-page">
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
            <div className="search-and-filter">
              <input
                type="text"
                placeholder="Tìm kiếm theo tên, mã BHXH, CCCD..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="search-input"
              />
              <div className="column-filter-container">
                <button 
                  className="column-filter-btn" 
                  onClick={() => setShowColumnFilter(!showColumnFilter)}
                  title="Chọn cột hiển thị"
                >
                  ⚙️ Cột
                </button>
                {showColumnFilter && (
                  <div className="column-filter-dropdown">
                    <div className="column-filter-header">Hiển thị cột</div>
                    {Object.entries(columnLabels).map(([key, label]) => (
                      <label key={key} className="column-filter-item">
                        <input
                          type="checkbox"
                          checked={visibleColumns[key]}
                          onChange={() => handleColumnToggle(key)}
                        />
                        <span>{label}</span>
                      </label>
                    ))}
                  </div>
                )}
              </div>
            </div>
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
                    {visibleColumns.stt && <th>STT</th>}
                    {visibleColumns.name && <th>Họ và tên</th>}
                    {visibleColumns.gender && <th title="Giới tính">GT</th>}
                    {visibleColumns.socialInsurance && <th title="Mã số BHXH">Mã BHXH</th>}
                    {visibleColumns.dob && <th>Ngày sinh</th>}
                    {visibleColumns.identityCard && <th title="Căn cước công dân">CCCD</th>}
                    {visibleColumns.insuranceDate && <th title="Thời gian đóng BHXH">TG BHXH</th>}
                    {visibleColumns.position && <th title="Chuyên môn nghiệp vụ">Chuyên môn</th>}
                    {visibleColumns.profession && <th>Trình độ</th>}
                    {visibleColumns.grade && <th title="Bậc lương">Bậc</th>}
                    {visibleColumns.salary && <th>Tổng lương</th>}
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  {sortedEmployees.map((emp, index) => (
                    <tr key={emp.id}>
                      {visibleColumns.stt && <td className="stt-cell">{index + 1}</td>}
                      {visibleColumns.name && <td className="name-cell">{emp.fullName}</td>}
                      {visibleColumns.gender && <td className="gender-cell">{emp.sex === 0 ? 'Nữ' : 'Nam'}</td>}
                      {visibleColumns.socialInsurance && <td>{emp.socialInsurance}</td>}
                      {visibleColumns.dob && <td>{formatDate(emp.dob)}</td>}
                      {visibleColumns.identityCard && <td>{emp.identityCardNumber}</td>}
                      {visibleColumns.insuranceDate && <td>{formatMonthYear(emp.socialInsuranceContributionDate)}</td>}
                      {visibleColumns.position && <td className="position-cell">{emp.positionName}</td>}
                      {visibleColumns.profession && <td className="profession-cell">{emp.professionName}</td>}
                      {visibleColumns.grade && <td className="grade-cell">{emp.currentSalaryGrade}</td>}
                      {visibleColumns.salary && (
                        <td 
                          className="salary-cell clickable"
                          onClick={() => handleShowSalaryDetails(emp)}
                          title="Nhấn để xem chi tiết lương"
                        >
                          <span className="salary-amount">{formatCurrency(emp.totalSalary)}</span>
                          <span className="salary-hint">VNĐ</span>
                        </td>
                      )}
                      <td className="action-cell">
                        <button
                          className="btn-menu"
                          onClick={(e) => handleMenuToggle(e, emp.id)}
                          title="Thao tác"
                        >⋮</button>
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

      {/* Action Dropdown Menu */}
      {openMenuId && (() => {
        const emp = sortedEmployees.find(e => e.id === openMenuId);
        return emp ? (
          <div className="action-dropdown" style={{ top: menuPos.top, right: menuPos.right }}>
            <button className="action-item action-item--delete" onClick={() => handleDelete(emp)}>Xoá</button>
          </div>
        ) : null;
      })()}

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
