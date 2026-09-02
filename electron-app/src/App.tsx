import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider, useAuth } from './lib/auth';
import { Layout } from './components/Layout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Patients from './pages/Patients';
import Appointments from './pages/Appointments';
import Scribe from './pages/Scribe';
import Discharge from './pages/Discharge';
import Revenue from './pages/Revenue';
import Claims from './pages/Claims';
import Pharmacy from './pages/Pharmacy';
import Lab from './pages/Lab';
import Beds from './pages/Beds';
import Admin from './pages/Admin';

const qc = new QueryClient();

function Protected({ children }: { children:any }){
  const { user, loading } = useAuth();
  if(loading) return <div className="p-8 text-sm text-slate-500">Loading…</div>;
  if(!user) return <Navigate to="/login" replace />;
  return children;
}

function AppRoutes(){
  const { user } = useAuth();
  return (
    <Routes>
      <Route path="/login" element={<Login/>} />
      <Route path="/" element={<Protected><Layout><Dashboard/></Layout></Protected>} />
      <Route path="/patients" element={<Protected><Layout><Patients/></Layout></Protected>} />
      <Route path="/appointments" element={<Protected><Layout><Appointments/></Layout></Protected>} />
      <Route path="/scribe" element={<Protected><Layout><Scribe/></Layout></Protected>} />
      <Route path="/discharge" element={<Protected><Layout><Discharge/></Layout></Protected>} />
      <Route path="/revenue" element={<Protected><Layout><Revenue/></Layout></Protected>} />
      <Route path="/claims" element={<Protected><Layout><Claims/></Layout></Protected>} />
      <Route path="/pharmacy" element={<Protected><Layout><Pharmacy/></Layout></Protected>} />
      <Route path="/lab" element={<Protected><Layout><Lab/></Layout></Protected>} />
      <Route path="/beds" element={<Protected><Layout><Beds/></Layout></Protected>} />
      <Route path="/admin" element={<Protected><Layout><Admin/></Layout></Protected>} />
      <Route path="*" element={<Navigate to={user?'/':'/login'} />} />
    </Routes>
  );
}

export default function App(){
  return (
    <QueryClientProvider client={qc}>
      <AuthProvider>
        <BrowserRouter>
          <AppRoutes/>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  );
}
