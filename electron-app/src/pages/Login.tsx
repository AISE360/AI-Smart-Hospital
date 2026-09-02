import { useState } from 'react';
import { useAuth } from '../lib/auth';
import { useNavigate } from 'react-router-dom';

export default function Login(){
  const { login } = useAuth();
  const nav = useNavigate();
  const [u,setU]=useState('admin');
  const [p,setP]=useState('Admin@123');
  const [mfa,setMfa]=useState('');
  const [err,setErr]=useState('');
  const [loading,setLoading]=useState(false);

  async function submit(e:any){
    e.preventDefault(); setErr(''); setLoading(true);
    try{ await login(u,p,mfa||undefined); nav('/'); } catch(ex:any){ setErr(ex.response?.data?.message || ex.message || 'Login failed'); }
    finally{ setLoading(false); }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-900 p-6">
      <div className="w-full max-w-5xl grid md:grid-cols-2 gap-6">
        <div className="bg-white rounded-2xl p-8 shadow-xl">
          <div className="text-xs font-bold tracking-widest text-blue-600">AI SMART HOSPITAL</div>
          <h1 className="text-2xl font-bold mt-2">Sign in to Hospital OS</h1>
          <p className="text-sm text-slate-500 mt-1">Electron desktop • RBAC • MFA for privileged roles • Audit-logged</p>
          <form onSubmit={submit} className="mt-6 space-y-4">
            <div><label className="text-xs font-semibold">Username</label><input value={u} onChange={e=>setU(e.target.value)} className="mt-1 w-full border rounded-lg px-3 py-2" /></div>
            <div><label className="text-xs font-semibold">Password</label><input type="password" value={p} onChange={e=>setP(e.target.value)} className="mt-1 w-full border rounded-lg px-3 py-2" /></div>
            <div><label className="text-xs font-semibold">MFA code (if enabled • use 123456)</label><input value={mfa} onChange={e=>setMfa(e.target.value)} placeholder="Optional" className="mt-1 w-full border rounded-lg px-3 py-2" /></div>
            {err && <div className="text-sm text-red-600 bg-red-50 border border-red-200 rounded p-2">{err}</div>}
            <button disabled={loading} className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2.5 rounded-lg font-semibold">{loading?'Signing in…':'Sign in'}</button>
          </form>
          <div className="mt-4 text-xs text-slate-500">Demo accounts: <span className="font-mono">admin/Admin@123, doctor1/Doctor@123, frontdesk/Front@123, billing/Bill@123, pharmacy/Pharm@123</span></div>
        </div>
        <div className="bg-slate-800 rounded-2xl p-8 text-slate-100 flex flex-col justify-center">
          <div className="text-lg font-semibold">AI layer on top of HMIS/EMR</div>
          <ul className="mt-4 space-y-2 text-sm text-slate-300">
            <li>• Reception & FAQ — 40–60% self-service</li>
            <li>• <b className="text-white">Medical Scribe — 20–30% doc time saved</b> (AI_DRAFT → clinician sign)</li>
            <li>• <b className="text-white">Revenue AI — ₹1–2L/month leakage flagged</b></li>
            <li>• Claims pre-check & denial analytics</li>
            <li>• Pharmacy expiry & stock-out prediction</li>
            <li>• Command Center with SignalR live KPIs + AI Insight</li>
          </ul>
          <div className="mt-6 p-3 bg-amber-500/10 border border-amber-500/30 rounded text-xs text-amber-200">⚠️ Every AI output is marked <b>AI DRAFT</b> until a clinician explicitly signs. No autonomous diagnosis/prescribing.</div>
          <div className="mt-4 text-xs text-slate-500">Backend: ASP.NET Core 8 • PostgreSQL • SignalR • IAiClient pluggable (OpenAI/Claude/Azure)</div>
        </div>
      </div>
    </div>
  );
}
