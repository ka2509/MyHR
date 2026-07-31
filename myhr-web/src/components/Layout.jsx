import { useEffect } from 'react';
import { useNavigate, Outlet, NavLink } from 'react-router-dom';
import './Layout.css';

function Layout() {
  const navigate = useNavigate();
  const user = JSON.parse(localStorage.getItem('user') || '{}');

  useEffect(() => {
    if (!localStorage.getItem('user')) {
      navigate('/login');
    }
  }, [navigate]);

  const handleLogout = () => {
    localStorage.removeItem('user');
    navigate('/login');
  };

  return (
    <div className="app-layout">
      <aside className="sidebar">
        <div className="sidebar-logo">MyHR</div>
        <nav className="sidebar-nav">
          <NavLink to="/employees" className={({ isActive }) => `nav-item${isActive ? ' active' : ''}`}>
            <span className="nav-icon">👥</span>
            <span className="nav-label">Nhân viên</span>
          </NavLink>
          <NavLink to="/tinh-luong" className={({ isActive }) => `nav-item${isActive ? ' active' : ''}`}>
            <span className="nav-icon">💰</span>
            <span className="nav-label">Tính lương</span>
          </NavLink>
        </nav>
      </aside>

      <div className="page-wrapper">
        <header className="top-header">
          <span className="welcome-text">Xin chào, <strong>{user.fullName}</strong></span>
          <button onClick={handleLogout} className="logout-btn">Đăng xuất</button>
        </header>
        <main className="page-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

export default Layout;
