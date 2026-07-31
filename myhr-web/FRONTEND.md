# MyHR – Frontend Reference

> **Purpose of this file**: Comprehensive reference for generating consistent, precise frontend code for the MyHR project. Read this before implementing any UI task.

---

## Tech Stack

| Tool | Version | Purpose |
|------|---------|---------|
| React | 19 | UI library |
| Vite | 6 | Build tool / dev server |
| react-router-dom | 7 | Client-side routing |
| axios | latest | HTTP client |

No UI component library. All styling is hand-written plain CSS (one `.css` file per component/page, co-located).

---

## Project Structure

```
src/
├── main.jsx                  # Entry point – mounts <App /> into #root
├── index.css                 # Global reset + body font
├── App.css                   # Intentionally empty
├── App.jsx                   # Router root – defines all routes
│
├── api/
│   ├── config.js             # Axios instance (baseURL from VITE_API_URL env var)
│   └── employeeApi.js        # All API call functions (one function per endpoint)
│
├── components/
│   ├── Layout.jsx            # Shared shell: sidebar + top header + <Outlet />
│   └── Layout.css
│
└── pages/
    ├── Login.jsx             # Public page – CCCD + password login form
    ├── Login.css
    ├── EmployeeList.jsx      # Default authenticated page – employee table
    ├── EmployeeList.css
    ├── AddEmployee.jsx       # Form to add a single new employee
    ├── AddEmployee.css
    ├── TinhLuong.jsx         # Placeholder – Salary Calculation page
    └── TinhLuong.css         # (to be created when feature is built)
```

---

## Routing

```
/login              → Login.jsx          (public, outside Layout)
/                   → redirect → /employees
/employees          → EmployeeList.jsx   (inside Layout)
/employees/add      → AddEmployee.jsx    (inside Layout)
/tinh-luong         → TinhLuong.jsx      (inside Layout)
```

All authenticated routes are children of `<Layout />` in App.jsx. Adding a new authenticated page = add a `<Route>` inside the Layout block and a `<NavLink>` in Layout.jsx.

---

## Authentication

- Login posts `{ identityCardNumber, password }` to `POST /api/Employees/Login`
- On success, the user object is stored in `localStorage` as key `'user'`
- `Layout.jsx` reads `localStorage.getItem('user')` on mount; redirects to `/login` if absent
- Individual pages also guard themselves with the same check (defensive double-check)
- Logout = `localStorage.removeItem('user')` + `navigate('/login')` — handled in Layout

**User object shape** (stored in localStorage, from API response):
```js
{
  id: string,
  fullName: string,
  sex: number,           // 0 = Female, 1 = Male
  identityCardNumber: string,
  socialInsurance: string,
  // … other employee fields
}
```

---

## Layout / Shell

**Files**: `src/components/Layout.jsx` + `Layout.css`

```
┌──────────────────────────────────────────────────────┐
│  SIDEBAR (220px)     │  TOP HEADER (white bar)        │
│  bg: #1a365d         │  welcome text + logout btn     │
│                      ├────────────────────────────────│
│  • 👥 Nhân viên      │  PAGE CONTENT (<Outlet />)     │
│  • 💰 Tính lương     │  bg: #f5f7fa                   │
│                      │                                │
└──────────────────────────────────────────────────────┘
```

- Sidebar: `position: sticky; height: 100vh` — stays fixed while content scrolls
- Active nav item: `border-left: 3px solid #63b3ed` + slightly lighter background
- Top header: `position: sticky; top: 0; z-index: 10`
- Content area: `flex: 1`, receives `<Outlet />` output

**To add a new nav item:**
1. Add a `<NavLink>` in the `<nav className="sidebar-nav">` block in `Layout.jsx`
2. Add a `<Route>` inside the Layout route block in `App.jsx`
3. Create the page file in `src/pages/`

---

## Design System

### Color Palette

| Token | Hex | Usage |
|-------|-----|-------|
| Brand dark | `#1a365d` | Sidebar bg, table header bg, h2 color |
| Brand mid | `#2d5a87` | Hover states, links |
| Brand light | `#63b3ed` | Active nav indicator, salary grade text |
| Page bg | `#f5f7fa` | Page/body background |
| Surface | `#ffffff` | Cards, table bg, modal bg, top header |
| Border | `#e2e8f0` | Table row borders, input borders, top header border |
| Text primary | `#333333` | Table cell text |
| Text secondary | `#4a5568` | Labels, secondary info, logout button |
| Text muted | `#64748b` | STT cells, counts, hints |
| Danger | `#e53e3e` | Delete action text/border |
| Danger hover bg | `#fff5f5` | Delete hover background |
| Success | `#16a34a` | Success messages |

### Typography

- Base font: `-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Arial, sans-serif` (set in `index.css`)
- Font sizes: `0.75rem` (small), `0.8rem` (table), `0.85rem` (secondary), `0.9rem` (body), `1rem` (default), `1.4rem` (logo/h1)

### Spacing

- Page content padding: `20px 16px` (set on `.employee-list-page` and similar page containers)
- Section gaps: `16–24px`
- Button padding: `6–8px 14–16px`

### Border Radius

- Buttons: `4px`
- Cards / table wrappers: `8px`
- Modals: `8px`
- Dropdowns: `6px`

---

## CSS Conventions

- **One `.css` file per component**, imported at top of the JSX file
- **No CSS modules** — flat global class names
- **No Tailwind** — all styles written in the co-located CSS file
- **Naming pattern**: `{page/component}-{element}`, kebab-case
  - Examples: `.employee-list-page`, `.employee-table`, `.salary-cell`, `.btn-add-employee`
- **Button class pattern**:
  - `.btn-add-employee` — primary action (dark blue bg)
  - `.btn-back` — secondary / ghost
  - `.btn-menu` — icon-only (⋮)
  - `button[type="submit"]` — form submit (full-width in Login/AddEmployee)
- **State classes**: `.error-message` (red), `.success-message` (green), `.loading` (centered full-page)

---

## API Layer

**`src/api/config.js`**
```js
const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5133/api';
const api = axios.create({ baseURL: API_BASE_URL, headers: { 'Content-Type': 'application/json' } });
export default api;
```

**`src/api/employeeApi.js`** — all exported functions return `response.data`:

| Function | Method | Endpoint |
|----------|--------|----------|
| `login(identityCardNumber, password)` | POST | `/Employees/Login` |
| `getMainOrganizations()` | GET | `/Organizations/main` |
| `getSubOrganizations(parentId)` | GET | `/Organizations/{parentId}/sub` |
| `getEmployeesByOrganization(orgId)` | GET | `/Employees/organization/{orgId}` |
| `getEmployeeById(employeeId)` | GET | `/Employees/{employeeId}` |
| `addEmployee(data)` | POST | `/Employees/add` |
| `deleteEmployee(employeeId)` | DELETE | `/Employees/{employeeId}` |
| `getAllPositions()` | GET | `/Positions` |
| `getAllProfessions()` | GET | `/Professions` |
| `getAllAllowances()` | GET | `/Allowances` |

**To add a new API call**: add an exported `async` function in `employeeApi.js` using the `api` axios instance.

---

## Page Reference

### Login (`/login`)

- **State**: `identityCardNumber`, `password`, `error`, `loading`
- On submit: calls `login()`, saves result to `localStorage('user')`, navigates to `/employees`
- Outside Layout shell — full-page centered card on gradient background (`linear-gradient(135deg, #1a365d, #2d5a87)`)

---

### EmployeeList (`/employees`)

**State:**

| State var | Type | Purpose |
|-----------|------|---------|
| `mainOrgs` | array | Top-level orgs (rendered as tab buttons) |
| `activeTab` | string | Selected main org ID |
| `subOrgs` | array | Sub-orgs for active tab |
| `selectedSubOrg` | `{id, name}` | Currently selected sub-org |
| `employees` | array | Raw employee list from API |
| `searchTerm` | string | Filter input value |
| `showSalaryModal` | bool | Salary detail modal open/closed |
| `selectedEmployee` | object\|null | Employee shown in salary modal |
| `openMenuId` | string\|null | Row ID whose ⋮ menu is open |
| `menuPos` | `{top, right}` | Fixed pixel coords for action dropdown |

**Data flow:**
1. Mount → `getMainOrganizations()` → auto-loads first tab's sub-orgs
2. Tab click → `getSubOrganizations(orgId)` → clears employees
3. Sub-org select → `getEmployeesByOrganization(orgId)`
4. Display: filter by `searchTerm`, then sort descending by `totalSalary`

**Employee object shape** (from API):
```js
{
  id: string,
  fullName: string,
  sex: number,                          // 0=Female, 1=Male
  socialInsurance: string,
  dob: string,                          // ISO date string
  identityCardNumber: string,
  socialInsuranceContributionDate: string,
  positionId: string,
  positionName: string,
  professionId: string,
  professionName: string,
  allowanceId: string | null,
  allowanceName: string | null,
  allowanceCoefficient: number | null,
  currentSalaryGrade: number | null,    // Bậc lương (1–12)
  salaryCof: number | null,             // Hệ số lương
  totalSalary: number,                  // VNĐ
}
```

**Table columns** (12): STT | Họ và tên | GT | Mã BHXH | Ngày sinh | CCCD | TG BHXH | Chuyên môn | Trình độ | Bậc | Tổng lương | ⋮

**Action dropdown (⋮ menu) — important pattern:**
- Uses `position: fixed` to escape the `overflow-x: auto` table wrapper (which would clip `absolute` children)
- Position calculated from `e.currentTarget.getBoundingClientRect()` at click time, stored in `menuPos`
- Dropdown rendered **at component root level** (outside the table), not inside the `<td>`
- Close on outside `mousedown` using `document.addEventListener` + `e.target.closest()` check
- To add a new action item: add `<button className="action-item">` inside the dropdown div

**Salary modal:** Opens on click of any salary cell. Pattern: overlay click closes, inner click stops propagation.

---

### AddEmployee (`/employees/add`)

- Loads reference data on mount: positions, professions, allowances, main orgs
- Two-step org selection: main org → sub-org (dependent dropdowns)
- Password auto-set to employee's CCCD number
- On success: shows success message, then navigates to `/employees`
- On cancel: navigates to `/employees`
- Has its own page-level `<div className="add-employee-header">` with title + back button

---

### TinhLuong (`/tinh-luong`)

- Currently a placeholder (feature to be defined)
- File: `src/pages/TinhLuong.jsx`

---

## Reusable Patterns

### Loading state
```jsx
if (loading) return <div className="loading">Đang tải...</div>;
```

### Inline error/success
```jsx
{error && <div className="error-message">{error}</div>}
{success && <div className="success-message">{success}</div>}
```

### Modal (overlay + content)
```jsx
<div className="modal-overlay" onClick={onClose}>
  <div className="modal-content" onClick={e => e.stopPropagation()}>
    <div className="modal-header">
      <h3>Title</h3>
      <button className="modal-close-btn" onClick={onClose}>×</button>
    </div>
    <div className="modal-body">...</div>
  </div>
</div>
```

### Date & currency formatting
```js
// dd/mm/yyyy (Vietnamese locale)
date.toLocaleDateString('vi-VN')

// m/yyyy
`${date.getMonth() + 1}/${date.getFullYear()}`

// 1.234.567 (Vietnamese thousand separators)
new Intl.NumberFormat('vi-VN').format(amount)
```

### Action dropdown (fixed position, escapes overflow)
```jsx
// In state: const [openMenuId, setOpenMenuId] = useState(null);
//           const [menuPos, setMenuPos] = useState({ top: 0, right: 0 });

// Toggle handler
const handleMenuToggle = (e, id) => {
  if (openMenuId === id) { setOpenMenuId(null); return; }
  const rect = e.currentTarget.getBoundingClientRect();
  setMenuPos({ top: rect.bottom + 4, right: window.innerWidth - rect.right });
  setOpenMenuId(id);
};

// Close on outside click (in useEffect)
document.addEventListener('mousedown', (e) => {
  if (!e.target.closest('.btn-menu') && !e.target.closest('.action-dropdown'))
    setOpenMenuId(null);
});

// Button in table cell
<button className="btn-menu" onClick={(e) => handleMenuToggle(e, item.id)}>⋮</button>

// Dropdown at component root (outside table)
{openMenuId && (
  <div className="action-dropdown" style={{ top: menuPos.top, right: menuPos.right }}>
    <button className="action-item action-item--delete" onClick={handleDelete}>Xoá</button>
  </div>
)}
```

---

## Environment Variables

| Variable | Purpose |
|----------|---------|
| `VITE_API_URL` | Full API base URL including `/api`. E.g. `https://myhr.up.railway.app/api` |

Local development — create `.env.local` (gitignored):
```
VITE_API_URL=http://localhost:5133/api
```

Production — set in Vercel dashboard → Project → Settings → Environment Variables.

---

## Build & Deploy

- **Dev server**: `npm run dev` → `http://localhost:5173`
- **Build**: `npm run build` → outputs to `dist/`
- **Deployed on**: Vercel (root directory: `myhr-web`)
- `vercel.json` rewrites all paths to `index.html` for SPA client-side routing
- Auto-deploys on every push to `main` branch of the GitHub repo
