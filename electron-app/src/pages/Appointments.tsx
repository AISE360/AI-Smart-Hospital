import { useEffect, useState } from 'react';
import { api } from '../lib/api';

export default function Appointments(){
  const [list,setList]=useState<any[]>([]);
  const [patients,setPatients]=useState<any[]>([]);
  const [form,setForm]=useState<any>({patientId:'', doctorId:'', scheduledAt:'', reason:''});
  const [faqQ,setFaqQ]=useState('What are OPD timings?'); const [faqA,setFaqA]=useState('');
  const [slots,setSlots]=useState<any[]>([]);

  async function load(){ const r=await api.get('/appointments'); setList(r.data); const p=await api.get('/patients'); setPatients(p.data); }
  useEffect(()=>{ load(); },[]);

  async function book(e:any){
    e.preventDefault();
    try{ await api.post('/appointments', { patientId:form.patientId, doctorId:form.doctorId, scheduledAt:new Date(form.scheduledAt).toISOString(), reason:form.reason }); load(); } catch(ex:any){ alert(ex.response?.data||ex.message); }
  }
  async function askFaq(){ const r=await api.post('/appointments/faq', {question:faqQ}); setFaqA(r.data.answer); }
  async function checkAvail(){
    if(!form.doctorId) return alert('Enter doctorId');
    const r=await api.get('/appointments/availability', {params:{doctorId:form.doctorId, date:new Date(form.scheduledAt||new Date()).toISOString()}});
    setSlots(r.data);
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Appointments & Reception <span className="text-xs bg-blue-100 text-blue-700 px-2 py-1 rounded">AI FAQ • Reminders</span></h1>
      <div className="grid md:grid-cols-3 gap-6">
        <div className="md:col-span-2 space-y-4">
          <div className="bg-white rounded-xl border p-4">
            <div className="text-sm font-semibold">Today's Queue</div>
            <div className="mt-2 divide-y max-h-72 overflow-auto">
              {list.map((a:any)=>(
                <div key={a.id} className="py-2 flex items-center justify-between text-sm">
                  <div><span className="font-bold">{a.tokenNumber}</span> {a.patientName} → <span className="font-medium">{a.doctorName||a.doctorId.slice(0,6)}</span> <span className="text-xs bg-slate-100 border px-1 rounded">{a.status}</span></div>
                  <span className="text-xs text-slate-500">{new Date(a.scheduledAt).toLocaleString('en-IN')}</span>
                </div>
              ))}
            </div>
          </div>
          <div className="bg-white rounded-xl border p-4">
            <div className="text-sm font-semibold">Book Appointment</div>
            <form onSubmit={book} className="mt-3 grid md:grid-cols-2 gap-3">
              <select value={form.patientId} onChange={e=>setForm({...form,patientId:e.target.value})} className="border rounded px-3 py-2" required><option value="">Select patient</option>{patients.map((p:any)=><option key={p.id} value={p.id}>{p.fullName} ({p.mrn})</option>)}</select>
              <input placeholder="Doctor Id (copy from /patients or use doctor1 id)" value={form.doctorId} onChange={e=>setForm({...form,doctorId:e.target.value})} className="border rounded px-3 py-2" required />
              <input type="datetime-local" value={form.scheduledAt} onChange={e=>setForm({...form,scheduledAt:e.target.value})} className="border rounded px-3 py-2" required />
              <input placeholder="Reason" value={form.reason} onChange={e=>setForm({...form,reason:e.target.value})} className="border rounded px-3 py-2" />
              <button className="md:col-span-2 bg-blue-600 text-white py-2 rounded">Book</button>
            </form>
            <button onClick={checkAvail} className="mt-2 text-xs border px-3 py-1.5 rounded">Check Availability</button>
            {slots.length>0 && <div className="mt-2 grid grid-cols-4 gap-1 text-xs">{slots.slice(0,16).map((s:any)=><span key={s.time} className={`px-2 py-1 rounded border ${s.available?'bg-green-50 border-green-200':'bg-red-50 border-red-200'}`}>{new Date(s.time).toLocaleTimeString([], {hour:'2-digit', minute:'2-digit'})}</span>)}</div>}
          </div>
        </div>
        <div className="bg-white rounded-xl border p-4">
          <div className="text-sm font-semibold">AI FAQ Assistant</div>
          <p className="text-xs text-slate-500">IAiClient • hospital FAQ knowledge base — no hallucinated medical advice</p>
          <div className="mt-3 flex gap-2"><input value={faqQ} onChange={e=>setFaqQ(e.target.value)} className="flex-1 border rounded px-3 py-2 text-sm" /><button onClick={askFaq} className="bg-slate-900 text-white px-3 py-2 rounded text-sm">Ask</button></div>
          {faqA && <div className="mt-3 p-3 bg-blue-50 border border-blue-200 rounded text-sm">{faqA}</div>}
          <div className="mt-4 text-xs text-slate-500">KPI: self-service rate, call deflection (stub). Real telephony out-of-scope for MVP.</div>
        </div>
      </div>
    </div>
  );
}
