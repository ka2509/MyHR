import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import EmployeeList from './pages/EmployeeList';
import AddEmployee from './pages/AddEmployee';
import TinhLuong from './pages/TinhLuong';
import Layout from './components/Layout';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route element={<Layout />}>
          <Route path="/employees" element={<EmployeeList />} />
          <Route path="/employees/add" element={<AddEmployee />} />
          <Route path="/tinh-luong" element={<TinhLuong />} />
          <Route path="/" element={<Navigate to="/employees" replace />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
