import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { login } from '../api/employeeApi';
import './Login.css';

function Login() {
  const [identityCardNumber, setIdentityCardNumber] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const user = await login(identityCardNumber, password);
      localStorage.setItem('user', JSON.stringify(user));
      navigate('/employees');
    } catch (err) {
      setError(err.response?.data?.message || 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-box">
        <h1>MyHR</h1>
        <h2>Đăng nhập</h2>
        
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="identityCardNumber">Số CCCD</label>
            <input
              type="text"
              id="identityCardNumber"
              value={identityCardNumber}
              onChange={(e) => setIdentityCardNumber(e.target.value)}
              placeholder="Nhập số CCCD"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Mật khẩu</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Nhập mật khẩu"
              required
            />
          </div>

          {error && <div className="error-message">{error}</div>}

          <button type="submit" disabled={loading}>
            {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
          </button>
        </form>
      </div>
    </div>
  );
}

export default Login;
