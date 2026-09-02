import { useEffect, useState } from 'react';
import { api } from '../lib/api';

export default function Admin(){
  const [flags,setFlags]=useState<any[]>([]);
  const [audit,setAudit]=useState<any[]>([]);
  const [aiLogs,setAiLogs]=useState<any[]>([]);

  async function load(){
    const [f,a,ai]=await Promise.all([api.get('/featureflags'), api.get('/audit'), api.get('/audit/ai-outputs')]);
    setFlags(f.data); setAudit(a.data); setAiLogs(ai.data);
  }
  useEffect(()=>{ load(); },[]);
  async function toggle(k:string){ await api.patch(`/featureflags/${k}/toggle`); load(); }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Admin & Governance</h1>
      <div className="grid md:grid-cols-2 gap-6">
        <div className="bg-white rounded-xl border p-4">
          <div className="text-sm font-semibold">Feature Flags (progressive enable)</div>
          <div className="mt-3 space-y-2">
            {flags.map((f:any)=><div key={f.key} className="flex items-center justify-between border rounded px-3 py-2"><span className="text-sm">{f.displayName}<span className="text-xs text-slate-500 ml-2">{f.key}</span></span><button onClick={()=>toggle(f.key)} className={`text-xs px-3 py-1 rounded ${f.isEnabled?'bg-green-600 text-white':'bg-slate-200'}`}>{f.isEnabled?'Enabled':'Disabled'}</button></div>)}
          </div>
        </div>
        <div className="bg-white rounded-xl border p-4">
          <div className="text-sm font-semibold">AI Output Governance</div>
          <div className="text-xs text-slate-500">Model/prompt/version tracking • Approval gate • Override tracking</div>
          <div className="mt-3 divide-y max-h-72 overflow-auto">
            {aiLogs.slice(0,8).map((l:any)=><div key={l.id} className="py-2 text-xs"><div className="font-mono">{l.taskType} • {l.modelName} v{l.modelVersion} • {l.status}</div><div className="text-slate-500 truncate">{l.inputSummary}</div></div>)}
          </div>
        </div>
      </div>
      <div className="bg-white rounded-xl border">
        <div className="p-4 border-b text-sm font-semibold">Immutable Audit Log (append-only)</div>
        <div className="divide-y max-h-96 overflow-auto">
          {audit.map((a:any)=><div key={a.id} className="p-2 flex justify-between text-xs"><span>{new Date(a.timestamp).toLocaleString()} • {a.userName} ({a.userRole}) • {a.action} {a.entityType}:{a.entityId.slice(0,6)}</span><span className={a.isSensitive?'text-red-600':''}>{a.isSensitive?'sensitive':''}</span></div>)}
        </div>
      </div>
      <div className="bg-slate-900 text-slate-100 rounded-xl p-4 text-xs">
        <div className="font-bold">Governance: Human-approval gates</div>
        <ul className="list-disc ml-4 mt-2 space-y-1 text-slate-300">
          <li>Diagnosis, prescription, discharge sign-off can never auto-finalize — explicit “Approve/Sign” required.</li>
          <li>Every AI output tagged AI_DRAFT until approved (badge + DB status + AiOutputLog).</li>
          <li>Signed notes are immutable — edits create new versioned entry.</li>
          <li>Accuracy/override tracking: edits/rejections logged to measure acceptance rate (&gt;85% target).</li>
          <li>FHIR/ABDM-ready, SNOMED/LOINC/ICD-10 nullable coding, ConsentRecord, DPDP data minimization.</li>
        </ul>
      </div>
    </div>
  );
}
