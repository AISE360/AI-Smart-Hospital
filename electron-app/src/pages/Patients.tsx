import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { Search, Users, UserPlus, Shield } from 'lucide-react';

export default function Patients(){
  const [q,setQ]=useState(''); const [list,setList]=useState<any[]>([]); const [form,setForm]=useState<any>({FullName:'', Gender:'Male', DateOfBirth:'1990-01-01', Phone:'', Email:''}); const [msg,setMsg]=useState('');

  async function search(){ const r=await api.get('/patients', {params:{search:q}}); setList(r.data); }
  useEffect(()=>{ search(); },[]);

  async function create(e:any){
    e.preventDefault();
    try{ const r=await api.post('/patients', { FullName:form.FullName, Gender:form.Gender, DateOfBirth:new Date(form.DateOfBirth).toISOString(), Phone:form.Phone, Email:form.Email }); setMsg('✓ Created '+r.data.mrn); search(); } catch(ex:any){ setMsg(ex.response?.data||ex.message); }
  }

  return (
    <div className="space-y-6 animate-fade-in">
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-600 to-indigo-600 flex items-center justify-center text-white shadow-glow"><Users className="w-5 h-5" /></div>
        <div>
          <h1 className="text-[26px] font-extrabold tracking-tight">Patients</h1>
          <p className="text-sm text-slate-500 -mt-1">ABDM • FHIR-ready • Aadhaar hashed • DPDP • ABHA</p>
        </div>
        <span className="ml-auto hidden sm:inline-flex items-center gap-1.5 text-xs font-bold bg-slate-900 text-white px-3 py-1.5 rounded-full"><Shield className="w-3.5 h-3.5" /> Consent-tracked</span>
      </div>

      <div className="bg-white rounded-2xl border border-slate-200/70 shadow-soft p-4 flex gap-3">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
          <input value={q} onChange={e=>setQ(e.target.value)} placeholder="Search by name, MRN or phone..." className="w-full bg-slate-50 border border-slate-200 rounded-xl pl-10 pr-3 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition" />
        </div>
        <button onClick={search} className="bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white px-6 py-3 rounded-xl font-bold shadow-glow flex items-center gap-2 transition"><Search className="w-4 h-4" /> Search</button>
      </div>

      <div className="grid lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 bg-white rounded-2xl border border-slate-200/70 shadow-soft overflow-hidden">
          <div className="p-5 border-b border-slate-100 flex items-center justify-between">
            <div className="text-sm font-extrabold">Results</div>
            <span className="text-xs font-bold bg-slate-900 text-white px-2.5 py-1 rounded-full">{list.length} patients</span>
          </div>
          <div className="divide-y divide-slate-100 max-h-[520px] overflow-auto">
            {list.map(p=>(
              <div key={p.id} className="p-4 flex items-center justify-between hover:bg-slate-50/80 transition">
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-slate-800 to-slate-900 text-white flex items-center justify-center font-bold text-sm">{p.fullName[0]}</div>
                  <div>
                    <div className="font-semibold text-sm flex items-center gap-2">{p.fullName} <span className="text-xs bg-slate-100 border border-slate-200 px-2 py-0.5 rounded-full font-mono">{p.mrn}</span></div>
                    <div className="text-xs text-slate-500">{p.gender} • {new Date(p.dateOfBirth).toLocaleDateString()} • {p.phone}</div>
                  </div>
                </div>
                <span className="hidden sm:inline text-xs text-slate-500 bg-slate-50 border px-2 py-1 rounded-full">{p.email}</span>
              </div>
            ))}
            {list.length===0 && <div className="p-12 text-center text-sm text-slate-500">No patients — try a different search or register.</div>}
          </div>
        </div>
        <div className="bg-white rounded-2xl border border-slate-200/70 shadow-soft p-5">
          <div className="flex items-center gap-2 font-extrabold"><UserPlus className="w-4 h-4 text-blue-600" /> Quick Registration</div>
          <p className="text-xs text-slate-500 mt-1">Creates MRN + FHIR Patient + ConsentRecord (Treatment).</p>
          <form onSubmit={create} className="mt-4 space-y-3">
            <input placeholder="Full name" value={form.FullName} onChange={e=>setForm({...form,FullName:e.target.value})} className="w-full bg-slate-50 border border-slate-200 rounded-xl px-3.5 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition" required />
            <div className="grid grid-cols-2 gap-3">
              <select value={form.Gender} onChange={e=>setForm({...form,Gender:e.target.value})} className="bg-slate-50 border border-slate-200 rounded-xl px-3 py-3 text-sm"><option>Male</option><option>Female</option><option>Other</option></select>
              <input type="date" value={form.DateOfBirth} onChange={e=>setForm({...form,DateOfBirth:e.target.value})} className="bg-slate-50 border border-slate-200 rounded-xl px-3 py-3 text-sm" />
            </div>
            <input placeholder="Phone" value={form.Phone} onChange={e=>setForm({...form,Phone:e.target.value})} className="w-full bg-slate-50 border border-slate-200 rounded-xl px-3.5 py-3 text-sm focus:bg-white transition" />
            <input placeholder="Email" value={form.Email} onChange={e=>setForm({...form,Email:e.target.value})} className="w-full bg-slate-50 border border-slate-200 rounded-xl px-3.5 py-3 text-sm focus:bg-white transition" />
            <button className="w-full bg-slate-900 hover:bg-black text-white py-3 rounded-xl font-bold shadow-sm transition">Register</button>
            {msg && <div className="text-xs font-medium bg-emerald-50 text-emerald-800 border border-emerald-200 p-3 rounded-xl">{msg}</div>}
          </form>
        </div>
      </div>
    </div>
  );
}
