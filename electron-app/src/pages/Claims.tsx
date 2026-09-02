import { useEffect, useState } from 'react';
import { api } from '../lib/api';

export default function Claims(){
  const [claims,setClaims]=useState<any[]>([]);
  const [analytics,setAnalytics]=useState<any>(null);
  const [precheck,setPrecheck]=useState<any>(null);

  async function load(){ const c=await api.get('/claims'); setClaims(c.data); const a=await api.get('/claims/denials/analytics'); setAnalytics(a.data); }
  useEffect(()=>{ load(); },[]);

  async function doPrecheck(id:string){
    const r=await api.post(`/claims/${id}/precheck`);
    setPrecheck(r.data);
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Insurance — Pre-check & Denial Analytics</h1>
      <div className="grid md:grid-cols-3 gap-4">
        <div className="bg-white rounded-xl border p-4"><div className="text-sm font-semibold">Total Denials</div><div className="text-2xl font-bold">{analytics?.totalDenials ?? '—'}</div><div className="text-xs text-slate-500">Denied ₹{analytics?.totalDeniedAmount}</div></div>
        <div className="bg-white rounded-xl border p-4"><div className="text-sm font-semibold">Rejection Drivers</div>{analytics?.byReason?.slice(0,3).map((r:any)=><div key={r.reason} className="text-xs flex justify-between"><span>{r.reason}</span><span>{r.count}</span></div>)}</div>
        <div className="bg-white rounded-xl border p-4"><div className="text-sm font-semibold">By Payer</div>{analytics?.byPayer?.slice(0,3).map((p:any)=><div key={p.payer} className="text-xs flex justify-between"><span>{p.payer}</span><span>₹{p.amount} ({p.count})</span></div>)}</div>
      </div>

      <div className="bg-white rounded-xl border">
        <div className="p-4 border-b flex items-center justify-between"><span className="text-sm font-semibold">Claims ({claims.length})</span><span className="text-xs text-slate-500">Missing docs • Coding mismatches • Payer-specific checks</span></div>
        <div className="divide-y">
          {claims.map((c:any)=>(
            <div key={c.id} className="p-3 flex items-center justify-between">
              <div><div className="text-sm font-medium">{c.claimNumber} — {c.payerName} <span className="text-xs border px-1 rounded">{c.status}</span></div><div className="text-xs text-slate-500">₹{c.claimedAmount} • {c.flags?.length||0} flags</div></div>
              <button onClick={()=>doPrecheck(c.id)} className="text-xs border px-3 py-1 rounded">Run Pre-check</button>
            </div>
          ))}
        </div>
      </div>
      {precheck && <div className={`p-4 rounded-xl border ${precheck.passed?'bg-green-50 border-green-200':'bg-red-50 border-red-200'}`}><div className="font-semibold">{precheck.passed?'✓ PASSED':'✗ ISSUES FOUND'}</div><div className="text-sm">Issues: {precheck.issues?.join(', ')||'none'}</div><div className="text-sm">Warnings: {precheck.warnings?.join(', ')||'none'}</div></div>}
    </div>
  );
}
