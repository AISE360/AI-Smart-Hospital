import { useState } from 'react';
import { useAuth } from '../lib/auth';
import { useNavigate } from 'react-router-dom';
import { Activity, Shield, Sparkles, TrendingUp, Clock, Building2, ArrowRight, Eye, EyeOff } from 'lucide-react';

export default function Login(){
  const { login } = useAuth();
  const nav = useNavigate();
  const [u,setU]=useState('admin');
  const [p,setP]=useState('Admin@123');
  const [mfa,setMfa]=useState('');
  const [err,setErr]=useState('');
  const [loading,setLoading]=useState(false);
  const [show,setShow]=useState(false);

  async function submit(e:any){
    e.preventDefault(); setErr(''); setLoading(true);
    try{ await login(u,p,mfa||undefined); nav('/'); } catch(ex:any){ setErr(ex.response?.data?.message || ex.response?.data || ex.message || 'Login failed'); }
    finally{ setLoading(false); }
  }

  return (
    <div className="min-h-screen bg-[#0f172a] flex items-center justify-center p-4 lg:p-6">
      <div className="absolute inset-0 bg-gradient-to-br from-blue-600 via-indigo-600 to-violet-700 opacity-90" />
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_30%_20%,rgba(255,255,255,0.15),transparent_50%),radial-gradient(circle_at_80%_80%,rgba(255,255,255,0.08),transparent_50%)]" />
      <div className="relative w-full max-w-6xl grid lg:grid-cols-[1.1fr_0.9fr] gap-0 bg-white rounded-[28px] shadow-[0_32px_80px_rgba(0,0,0,0.35)] overflow-hidden animate-fade-in">
        {/* Left - branding */}
        <div className="relative bg-gradient-to-br from-slate-900 via-slate-900 to-slate-800 p-8 lg:p-10 text-white overflow-hidden">
          <div className="absolute -top-24 -right-24 w-96 h-96 bg-blue-600/20 rounded-full blur-3xl" />
          <div className="absolute -bottom-24 -left-24 w-96 h-96 bg-indigo-500/15 rounded-full blur-3xl" />
          <div className="relative">
            <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-white/10 border border-white/20 text-xs font-bold tracking-widest">
              <span className="w-2 h-2 bg-emerald-400 rounded-full animate-pulse" /> HMIS AI LAYER • 50-BED • PUNE
            </div>
            <div className="mt-6 flex items-center gap-3">
              <div className="w-12 h-12 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center shadow-glow">
                <Activity className="w-7 h-7 text-white" />
              </div>
              <div>
                <div className="text-xl font-extrabold tracking-tight">AI Smart Hospital</div>
                <div className="text-xs text-slate-400">Hospital OS • Electron Desktop</div>
              </div>
            </div>
            <h2 className="mt-8 text-[28px] font-extrabold leading-tight tracking-tight">AI recommends.<br/><span className="bg-gradient-to-r from-blue-400 to-indigo-400 bg-clip-text text-transparent">Humans approve.</span></h2>
            <p className="mt-3 text-sm text-slate-300 leading-relaxed">Pluggable AI layer on top of HMIS/EMR, billing, LIS, RIS/PACS & pharmacy. Every clinical output is <b className="text-white">AI_DRAFT</b> until a clinician signs.</p>

            <div className="mt-8 grid grid-cols-2 gap-3">
              {[
                {k:'₹1–2L/mo', v:'Revenue leakage flagged', c:'from-red-500 to-orange-500', i: TrendingUp},
                {k:'20–30%', v:'Doc time saved (Scribe)', c:'from-emerald-500 to-teal-500', i: Clock},
                {k:'40–60%', v:'Self-service (Reception)', c:'from-blue-500 to-indigo-500', i: Sparkles},
                {k:'>85%', v:'AI draft acceptance', c:'from-violet-500 to-purple-500', i: Shield},
              ].map(card=>{
                const Icon = card.i as any;
                return (
                  <div key={card.k} className="bg-white/10 backdrop-blur rounded-2xl p-4 border border-white/10">
                    <div className={`w-8 h-8 rounded-xl bg-gradient-to-br ${card.c} flex items-center justify-center`}><Icon className="w-4 h-4 text-white" /></div>
                    <div className="mt-2 text-lg font-extrabold">{card.k}</div>
                    <div className="text-xs text-slate-300 leading-tight">{card.v}</div>
                  </div>
                );
              })}
            </div>
            <div className="mt-8 flex items-center gap-2 text-xs text-slate-400"><Building2 className="w-4 h-4" /> ASP.NET Core 8 • PostgreSQL • SignalR • IAiClient • Audit-logged • DPDP • FHIR</div>
          </div>
        </div>

        {/* Right - form */}
        <div className="p-8 lg:p-10 bg-white flex flex-col justify-center">
          <div className="max-w-sm w-full mx-auto">
            <div className="text-xs font-bold tracking-[0.15em] text-blue-600">SECURE SIGN IN</div>
            <h1 className="text-[26px] font-extrabold tracking-tight mt-1">Welcome back</h1>
            <p className="text-sm text-slate-500 mt-1">RBAC • MFA for Admin/Billing • Immutable audit • Offline-tolerant</p>

            <form onSubmit={submit} className="mt-7 space-y-4">
              <div>
                <label className="text-xs font-bold tracking-wide text-slate-700">Username</label>
                <input value={u} onChange={e=>setU(e.target.value)} placeholder="admin" className="mt-1.5 w-full bg-slate-50 border border-slate-200 rounded-xl px-3.5 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition" />
              </div>
              <div>
                <label className="text-xs font-bold tracking-wide text-slate-700">Password</label>
                <div className="relative mt-1.5">
                  <input type={show?"text":"password"} value={p} onChange={e=>setP(e.target.value)} className="w-full bg-slate-50 border border-slate-200 rounded-xl px-3.5 py-3 pr-10 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition" />
                  <button type="button" onClick={()=>setShow(!show)} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600"><Eye className={`w-4 h-4 ${show?'hidden':''}`} /><EyeOff className={`w-4 h-4 ${show?'':'hidden'}`} /></button>
                </div>
              </div>
              <div>
                <label className="text-xs font-bold tracking-wide text-slate-700">MFA code <span className="font-normal text-slate-400">(use 123456 if prompted)</span></label>
                <input value={mfa} onChange={e=>setMfa(e.target.value)} placeholder="Optional" className="mt-1.5 w-full bg-slate-50 border border-slate-200 rounded-xl px-3.5 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition" />
              </div>
              {err && <div className="text-sm text-red-700 bg-red-50 border border-red-200 rounded-xl p-3 flex items-start gap-2"><Shield className="w-4 h-4 mt-0.5 shrink-0" /> <span>{err}</span></div>}
              <button disabled={loading} className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white py-3 rounded-xl font-bold shadow-glow flex items-center justify-center gap-2 transition disabled:opacity-60">
                {loading?'Signing in…':'Sign in'} <ArrowRight className="w-4 h-4" />
              </button>
            </form>

            <div className="mt-6 p-3 rounded-xl bg-slate-900 text-slate-200">
              <div className="text-xs font-bold tracking-widest">DEMO ACCOUNTS</div>
              <div className="mt-2 grid grid-cols-2 gap-1.5 text-xs font-mono">
                <span className="bg-white/10 rounded px-2 py-1">admin / Admin@123</span>
                <span className="bg-white/10 rounded px-2 py-1">doctor1 / Doctor@123</span>
                <span className="bg-white/10 rounded px-2 py-1">frontdesk / Front@123</span>
                <span className="bg-white/10 rounded px-2 py-1">billing / Bill@123</span>
              </div>
            </div>
            <div className="mt-4 text-center text-xs text-slate-400">Fictional seed data • No real PHI • DPDP & ABDM FHIR-ready</div>
          </div>
        </div>
      </div>
    </div>
  );
}
