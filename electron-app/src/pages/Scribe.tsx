import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { AiDraftBadge } from '../components/Badge';

export default function Scribe(){
  const [notes,setNotes]=useState<any[]>([]);
  const [patients,setPatients]=useState<any[]>([]);
  const [encounters,setEncounters]=useState<any[]>([]);
  const [form,setForm]=useState<any>({patientId:'', encounterId:'', rawTranscript:'Patient presents with fever 3 days, cough, mild breathlessness. No chest pain.', history:'', assessment:''});
  const [selected,setSelected]=useState<any>(null);
  const [msg,setMsg]=useState('');

  async function load(){ const r=await api.get('/clinicalnotes'); setNotes(r.data); const p=await api.get('/patients'); setPatients(p.data); }
  useEffect(()=>{ load(); },[]);
  async function loadEncounters(pid:string){
    if(!pid) return;
    const r=await api.get(`/patients/${pid}/encounters`);
    setEncounters(r.data);
  }

  async function createDraft(){
    const r=await api.post('/clinicalnotes/draft', {
      encounterId: form.encounterId, patientId: form.patientId,
      rawTranscript: form.rawTranscript, history: form.history, assessment: form.assessment,
    });
    setMsg('Draft created '+r.data.id); load();
  }
  async function genAi(id:string){
    const r=await api.post(`/clinicalnotes/${id}/generate-ai`, { transcript: form.rawTranscript, overwrite:true });
    setSelected(r.data.note); load();
  }
  async function sign(id:string){
    const r=await api.post(`/clinicalnotes/${id}/sign`);
    setMsg('Signed v'+r.data.note.version+' — immutable');
    load();
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">AI Medical Scribe <span className="text-xs font-normal text-slate-500">Record → Transcribe → IAiClient draft → Clinician sign (immutable)</span></h1>
      <div className="bg-amber-50 border border-amber-200 rounded-lg p-3 text-xs">Every AI output is <b>AI DRAFT</b> until explicitly signed. Signed notes are immutable; edits create a new versioned entry. Target acceptance &gt;85% after tuning.</div>

      <div className="grid md:grid-cols-3 gap-6">
        <div className="bg-white rounded-xl border p-4">
          <div className="text-sm font-semibold">New Consultation</div>
          <div className="mt-3 space-y-3">
            <select value={form.patientId} onChange={e=>{setForm({...form,patientId:e.target.value}); loadEncounters(e.target.value);}} className="w-full border rounded px-3 py-2"><option value="">Select patient</option>{patients.map((p:any)=><option key={p.id} value={p.id}>{p.fullName}</option>)}</select>
            <select value={form.encounterId} onChange={e=>setForm({...form,encounterId:e.target.value})} className="w-full border rounded px-3 py-2"><option value="">Select encounter</option>{encounters.map((en:any)=><option key={en.id} value={en.id}>{en.chiefComplaint} ({new Date(en.startTime).toLocaleDateString()})</option>)}</select>
            <textarea value={form.rawTranscript} onChange={e=>setForm({...form,rawTranscript:e.target.value})} rows={4} placeholder="Transcript or typed notes" className="w-full border rounded px-3 py-2 text-sm"/>
            <button onClick={createDraft} className="w-full bg-blue-600 text-white py-2 rounded">Create Draft</button>
            {msg && <div className="text-xs text-green-700 bg-green-50 border p-2 rounded">{msg}</div>}
          </div>
        </div>

        <div className="md:col-span-2 bg-white rounded-xl border">
          <div className="p-4 border-b flex items-center justify-between"><span className="text-sm font-semibold">Notes ({notes.length})</span><span className="text-xs text-slate-500">Minutes saved/consult, correction rate KPI</span></div>
          <div className="divide-y max-h-[560px] overflow-auto">
            {notes.map((n:any)=>(
              <div key={n.id} className="p-4 hover:bg-slate-50">
                <div className="flex items-center justify-between"><span className="font-mono text-xs">{n.id.slice(0,8)} • v{n.version}</span><span className={`text-xs px-2 py-0.5 rounded border ${n.status.includes('Signed')?'bg-green-100 border-green-300 text-green-800':'bg-amber-100 border-amber-300'}`}>{n.status}</span></div>
                {n.isAiGenerated && <div className="mt-1"><AiDraftBadge/></div>}
                <div className="mt-2 text-sm line-clamp-2">{n.history || n.assessment || '—'}</div>
                <div className="mt-2 flex gap-2">
                  <button onClick={()=>genAi(n.id)} className="text-xs border px-2 py-1 rounded">Generate AI Draft</button>
                  <button onClick={()=>sign(n.id)} className="text-xs bg-slate-900 text-white px-2 py-1 rounded">Sign (approve)</button>
                  <button onClick={()=>setSelected(n)} className="text-xs border px-2 py-1 rounded">View</button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {selected && (
        <div className="bg-white rounded-xl border p-4">
          <div className="flex items-center justify-between"><span className="font-semibold">Selected Note {selected.id.slice(0,8)}</span><button onClick={()=>setSelected(null)} className="text-xs border px-2 py-1 rounded">Close</button></div>
          <pre className="mt-3 bg-slate-50 border rounded p-3 text-xs whitespace-pre-wrap overflow-auto">{JSON.stringify(selected,null,2)}</pre>
        </div>
      )}
    </div>
  );
}
