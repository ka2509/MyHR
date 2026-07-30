import api from './config';

export const login = async (identityCardNumber, password) => {
  const response = await api.post('/Employees/Login', {
    identityCardNumber,
    password,
  });
  return response.data;
};

export const getMainOrganizations = async () => {
  const response = await api.get('/Organizations/main');
  return response.data;
};

export const getSubOrganizations = async (parentId) => {
  const response = await api.get(`/Organizations/${parentId}/sub`);
  return response.data;
};

export const getEmployeesByOrganization = async (organizationId) => {
  const response = await api.get(`/Employees/organization/${organizationId}`);
  return response.data;
};

export const getEmployeeById = async (employeeId) => {
  const response = await api.get(`/Employees/${employeeId}`);
  return response.data;
};

export const addEmployee = async (employeeData) => {
  const response = await api.post('/Employees/add', employeeData);
  return response.data;
};

export const deleteEmployee = async (employeeId) => {
  const response = await api.delete(`/Employees/${employeeId}`);
  return response.data;
};

export const getAllPositions = async () => {
  const response = await api.get('/Positions');
  return response.data;
};

export const getAllProfessions = async () => {
  const response = await api.get('/Professions');
  return response.data;
};

export const getAllAllowances = async () => {
  const response = await api.get('/Allowances');
  return response.data;
};

