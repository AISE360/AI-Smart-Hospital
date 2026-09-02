import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { AiDraftBadge } from '../components/Badge';

export default function Discharge(){
  const [list,setList]=useState<any[]>([]);
  const [admissions,setAdmissions]=useState<any[]>([]);
  const [selectedAdmit,setSelectedAdmit]=useState('');
  const [course,setCourse]=useState('Patient admitted with pneumonia, treated with antibiotics, improved clinically. Vitals stable.');

  async function load(){ const r=await api.get('/dischargesummaries'); setList(r.data); const a=await api.get('/beds/admissions'); setAdmissions(a.data); }
  useEffect(()=>{ load(); },[]);

  async function generate(){
    if(!selectedAdmit) return alert('Select admission');
    const r=await api.post('/dischargesummaries/generate', { admissionId: selectedAdmit, admissionCourse: course });
    alert('Generated AI draft ' + r.data.summary.id);
    load();
  }
  async function approve(id:string){ await api.post(`/dischargesummaries/${id}/approve`); load(); }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">AI Discharge Summary <span className="text-xs font-normal text-slate-500">Pulls admission course → IAiClient draft → Doctor review & sign</span></h1>
      <div className="bg-white rounded-xl border p-4 grid md:grid-cols-3 gap-3">
        <select value={selectedAdmit} onChange={e=>setSelectedAdmit(e.target.value)} className="border rounded px-3 py-2 md:col-span-2"><option value="">Select admission</option>{admissions.map((a:any)=><option key={a.id} value={a.id}>{a.patient} — {a.bed} ({a.status})</option>)}</select>
        <button onClick={generate} className="bg-blue-600 text-white rounded px-4 py-2">Generate AI Draft</button>
        <textarea value={course} onChange={e=>setCourse(e.target.value)} rows={2} className="md:col-span-3 border rounded px-3 py-2 text-sm" placeholder="Admission course notes" />
      </div>
      <div className="bg-white rounded-xl border">
        <div className="p-4 border-b text-sm font-semibold">Summaries ({list.length})</div>
        <div className="divide-y">
          {list.map((s:any)=>(
            <div key={s.id} className="p-4">
              <div className="flex items-center justify-between"><span className="font-mono text-xs">{s.id.slice(0,8)} • v{s.version}</span><span className="text-xs border px-2 py-0.5 rounded bg-slate-50">{s.status}</span></div>
              {s.isAiGenerated && <div className="mt-1"><AiDraftBadge/></div>}
              <div className="mt-2 text-sm whitespace-pre-wrap line-clamp-3">{s.fullContent || s.admissionCourse}</div>
              <div className="mt-2"><button onClick={()=>approve(s.id)} className="text-xs bg-slate-900 text-white px-3 py-1 rounded">Approve & Sign</button></div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
