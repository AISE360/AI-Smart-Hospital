import { useEffect, useState } from 'react';
import { api } from '../lib/api';

export default function Revenue(){
  const [data,setData]=useState<any>(null);
  const [summary,setSummary]=useState<any>(null);
  const [running,setRunning]=useState(false);

  async function load(){
    const [l,s]=await Promise.all([api.get('/revenue/leaks'), api.get('/revenue/summary')]);
    setData(l.data); setSummary(s.data);
  }
  useEffect(()=>{ load(); },[]);

  async function run(){
    setRunning(true);
    try{ const r=await api.post('/revenue/run-reconciliation'); alert(`Found ${r.data.leaksFound} leaks • ₹${r.data.total}`); load(); } finally{ setRunning(false); }
  }

  if(!data) return <div className="text-sm text-slate-500">Loading revenue engine…</div>;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Revenue AI <span className="ml-2 text-xs bg-red-600 text-white px-2 py-1 rounded">FLAGSHIP — HIGHEST ROI</span></h1>
        <button onClick={run} disabled={running} className="bg-slate-900 text-white px-4 py-2 rounded-lg text-sm">{running?'Running…':'Run Reconciliation'}</button>
      </div>

      <div className="bg-gradient-to-r from-red-600 to-orange-600 rounded-2xl p-6 text-white shadow-lg">
        <div className="text-xs font-bold tracking-widest opacity-80">RECOVERABLE REVENUE (flagged, not necessarily collected)</div>
        <div className="mt-1 text-4xl font-black">₹ {(data.totalRecoverable/1000).toFixed(0)}K</div>
        <div className="text-sm opacity-90">{data.count} leaks • Avg ₹{data.count? (data.totalRecoverable/data.count).toFixed(0):0}/encounter • Target ₹1.0–2.0L/month</div>
        <div className={`mt-2 inline-flex px-2 py-1 rounded text-xs font-bold ${summary.status==='on_track'?'bg-green-500':'bg-amber-500'}`}>{summary.status==='on_track'?'ON TRACK':'ATTENTION'}</div>
      </div>

      <div className="grid md:grid-cols-3 gap-4">
        <div className="bg-white rounded-xl border p-4">
          <div className="text-sm font-semibold">Top Categories</div>
          <div className="mt-2 space-y-1">
            {(summary.topCategories||[]).map((c:any)=><div key={c.category} className="flex justify-between text-sm"><span>{c.category}</span><span className="font-bold">₹{c.amount} ({c.count})</span></div>)}
          </div>
        </div>
        <div className="bg-white rounded-xl border p-4">
          <div className="text-sm font-semibold">Leakage Alerts</div>
          <div className="text-3xl font-bold text-red-600">{data.count}</div>
          <div className="text-xs text-slate-500">Owner assignment & ageing surfaced on Command Center</div>
        </div>
        <div className="bg-white rounded-xl border p-4">
          <div className="text-sm font-semibold">Engine Rules</div>
          <ul className="text-xs text-slate-600 mt-2 list-disc ml-4 space-y-1">
            <li>Service performed but not billed</li>
            <li>Duplicate / incorrect charges</li>
            <li>Package exceptions</li>
            <li>Missing documentation</li>
          </ul>
        </div>
      </div>

      <div className="bg-white rounded-xl border">
        <div className="p-4 border-b text-sm font-semibold">Recoverable Queue</div>
        <div className="divide-y max-h-96 overflow-auto">
          {(data.items||[]).map((it:any)=>(
            <div key={it.serviceOrderId} className="p-3 flex items-center justify-between hover:bg-slate-50">
              <div><div className="font-medium text-sm">{it.serviceCode} — {it.serviceName} <span className="text-xs bg-red-50 border border-red-200 px-1 rounded">{it.category}</span></div><div className="text-xs text-slate-500">{it.patient} • {it.reason} • {it.encounterId?.slice(0,8)}</div></div>
              <div className="text-right"><div className="font-bold text-red-600">₹{it.leakageAmount}</div><div className="text-xs text-slate-500">{it.reason}</div></div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
