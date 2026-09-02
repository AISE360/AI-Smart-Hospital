import { useEffect, useState } from 'react';
import { api } from '../lib/api';

export default function Patients(){
  const [q,setQ]=useState(''); const [list,setList]=useState<any[]>([]); const [form,setForm]=useState<any>({FullName:'', Gender:'Male', DateOfBirth:'1990-01-01', Phone:'', Email:''}); const [msg,setMsg]=useState('');

  async function search(){ const r=await api.get('/patients', {params:{search:q}}); setList(r.data); }
  useEffect(()=>{ search(); },[]);

  async function create(e:any){
    e.preventDefault();
    try{ const r=await api.post('/patients', { FullName:form.FullName, Gender:form.Gender, DateOfBirth:new Date(form.DateOfBirth).toISOString(), Phone:form.Phone, Email:form.Email }); setMsg('Created '+r.data.mrn); search(); } catch(ex:any){ setMsg(ex.response?.data||ex.message); }
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Patients <span className="text-sm font-normal text-slate-500">ABDM • FHIR-ready • Aadhaar hashed • DPDP</span></h1>
      <div className="bg-white rounded-xl border p-4 flex gap-2">
        <input value={q} onChange={e=>setQ(e.target.value)} placeholder="Search by name/MRN/phone" className="flex-1 border rounded-lg px-3 py-2" />
        <button onClick={search} className="bg-blue-600 text-white px-4 py-2 rounded-lg">Search</button>
      </div>
      <div className="grid md:grid-cols-3 gap-6">
        <div className="md:col-span-2 bg-white rounded-xl border">
          <div className="p-4 border-b text-sm font-semibold">Results ({list.length})</div>
          <div className="divide-y max-h-[520px] overflow-auto">
            {list.map(p=>(
              <div key={p.id} className="p-3 flex items-center justify-between hover:bg-slate-50">
                <div><div className="font-medium">{p.fullName} <span className="text-xs bg-slate-100 border px-1.5 py-0.5 rounded">{p.mrn}</span></div><div className="text-xs text-slate-500">{p.gender} • {new Date(p.dateOfBirth).toLocaleDateString()} • {p.phone}</div></div>
                <span className="text-xs text-slate-400">{p.email}</span>
              </div>
            ))}
          </div>
        </div>
        <div className="bg-white rounded-xl border p-4">
          <div className="font-semibold">Quick Registration</div>
          <form onSubmit={create} className="mt-3 space-y-3">
            <input placeholder="Full name" value={form.FullName} onChange={e=>setForm({...form,FullName:e.target.value})} className="w-full border rounded px-3 py-2" required />
            <div className="grid grid-cols-2 gap-2">
              <select value={form.Gender} onChange={e=>setForm({...form,Gender:e.target.value})} className="border rounded px-2 py-2"><option>Male</option><option>Female</option><option>Other</option></select>
              <input type="date" value={form.DateOfBirth} onChange={e=>setForm({...form,DateOfBirth:e.target.value})} className="border rounded px-2 py-2" />
            </div>
            <input placeholder="Phone" value={form.Phone} onChange={e=>setForm({...form,Phone:e.target.value})} className="w-full border rounded px-3 py-2" />
            <input placeholder="Email" value={form.Email} onChange={e=>setForm({...form,Email:e.target.value})} className="w-full border rounded px-3 py-2" />
            <button className="w-full bg-slate-900 text-white py-2 rounded-lg">Register</button>
            {msg && <div className="text-xs text-green-700 bg-green-50 border p-2 rounded">{msg}</div>}
          </form>
        </div>
      </div>
    </div>
  );
}
