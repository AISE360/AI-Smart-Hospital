import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { IndianRupee, Zap, AlertTriangle, TrendingUp, Sparkles, Play } from 'lucide-react';

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
    try{ await api.post('/revenue/run-reconciliation'); load(); } finally{ setRunning(false); }
  }

  if(!data) return <div className="flex items-center justify-center h-48"><div className="w-8 h-8 rounded-full border-2 border-slate-200 border-t-blue-600 animate-spin" /></div>;

  return (
    <div className="space-y-6 animate-fade-in">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-gradient-to-r from-red-600 to-orange-600 text-white text-xs font-extrabold shadow"><Sparkles className="w-3.5 h-3.5" /> FLAGSHIP — HIGHEST ROI</div>
          <h1 className="text-[28px] font-extrabold tracking-tight mt-2">Revenue AI</h1>
          <p className="text-sm text-slate-500 -mt-1">Daily reconciliation • ServiceOrders vs InvoiceLines • “Recoverable” = flagged, not collected</p>
        </div>
        <button onClick={run} disabled={running} className="inline-flex items-center gap-2 bg-slate-900 hover:bg-black text-white px-5 py-2.5 rounded-xl text-sm font-bold shadow-soft transition disabled:opacity-60"><Play className="w-4 h-4" /> {running?'Running…':'Run Reconciliation'}</button>
      </div>

      <div className="relative overflow-hidden rounded-[24px] bg-gradient-to-br from-red-600 via-orange-600 to-amber-500 p-[1px] shadow-card">
        <div className="rounded-[23px] bg-gradient-to-br from-red-600 via-orange-600 to-amber-500 p-7 text-white relative overflow-hidden">
          <div className="absolute -top-10 -right-10 w-72 h-72 bg-white/15 rounded-full blur-3xl" />
          <div className="absolute -bottom-10 -left-10 w-72 h-72 bg-black/10 rounded-full blur-3xl" />
          <div className="relative">
            <div className="flex items-center gap-2 text-xs font-bold tracking-[0.15em] opacity-90"><IndianRupee className="w-4 h-4" /> RECOVERABLE REVENUE</div>
            <div className="mt-2 flex items-baseline gap-3">
              <span className="text-[42px] font-black tracking-tight">₹ {(data.totalRecoverable/1000).toFixed(0)}K</span>
              <span className={`px-3 py-1 rounded-full text-xs font-extrabold ${summary.status==='on_track'?'bg-emerald-500 text-white':'bg-white text-red-600'}`}>{summary.status==='on_track'?'● ON TRACK':'● ATTENTION'}</span>
            </div>
            <div className="mt-1 text-sm font-medium opacity-95">{data.count} leaks • Avg ₹{data.count? (data.totalRecoverable/data.count).toFixed(0):0}/encounter • Target ₹1.0–2.0L/month</div>
            <div className="mt-4 h-2 bg-white/20 rounded-full overflow-hidden"><div className="h-full bg-white rounded-full" style={{width: Math.min(100, (data.totalRecoverable/200000)*100)+'%'}} /></div>
          </div>
        </div>
      </div>

      <div className="grid md:grid-cols-3 gap-4">
        <div className="bg-white rounded-2xl border border-slate-200/70 shadow-soft p-5">
          <div className="flex items-center gap-2 text-sm font-extrabold"><Zap className="w-4 h-4 text-amber-500" /> Top Categories</div>
          <div className="mt-3 space-y-2">
            {(summary.topCategories||[]).map((c:any)=>(
              <div key={c.category} className="flex items-center justify-between p-2.5 rounded-xl bg-slate-50 border border-slate-200">
                <span className="text-sm font-medium">{c.category}</span>
                <span className="text-sm font-extrabold">₹{c.amount} <span className="text-xs font-normal text-slate-500">({c.count})</span></span>
              </div>
            ))}
          </div>
        </div>
        <div className="bg-white rounded-2xl border border-slate-200/70 shadow-soft p-5">
          <div className="text-sm font-extrabold flex items-center gap-2"><AlertTriangle className="w-4 h-4 text-red-500" /> Leakage Alerts</div>
          <div className="mt-2 text-4xl font-black text-red-600">{data.count}</div>
          <div className="text-xs text-slate-500 mt-1">Owner + ageing surfaced on Command Center. <span className="font-semibold">₹ recovered/month</span> is flagship KPI.</div>
          <div className="mt-3 inline-flex items-center gap-1 text-xs font-bold text-red-700 bg-red-50 border border-red-200 px-2.5 py-1 rounded-full"><TrendingUp className="w-3 h-3" /> Action queue</div>
        </div>
        <div className="bg-white rounded-2xl border border-slate-200/70 shadow-soft p-5">
          <div className="text-sm font-extrabold">Engine Rules</div>
          <ul className="mt-3 space-y-2 text-sm">
            {['Service performed but not billed','Duplicate / incorrect charges','Package exceptions','Missing documentation'].map(r=>(
              <li key={r} className="flex items-center gap-2"><span className="w-1.5 h-1.5 bg-blue-600 rounded-full" /> {r}</li>
            ))}
          </ul>
          <div className="mt-4 text-xs text-slate-500 bg-blue-50 border border-blue-200 rounded-xl p-2.5">Reconciliation is idempotent — safe to re-run daily via job queue (RabbitMQ/ServiceBus abstraction).</div>
        </div>
      </div>

      <div className="bg-white rounded-2xl border border-slate-200/70 shadow-soft overflow-hidden">
        <div className="p-5 border-b border-slate-100 flex items-center justify-between">
          <div className="text-sm font-extrabold">Recoverable Queue</div>
          <span className="text-xs bg-slate-900 text-white px-2.5 py-1 rounded-full">{data.count} items</span>
        </div>
        <div className="divide-y divide-slate-100 max-h-[420px] overflow-auto">
          {(data.items||[]).map((it:any)=>(
            <div key={it.serviceOrderId} className="p-4 flex items-center justify-between hover:bg-slate-50 transition">
              <div className="min-w-0">
                <div className="flex items-center gap-2">
                  <span className="font-bold text-sm">{it.serviceCode}</span>
                  <span className="text-sm text-slate-700 truncate">{it.serviceName}</span>
                  <span className="text-xs bg-red-50 text-red-700 border border-red-200 px-2 py-0.5 rounded-full font-bold">{it.category}</span>
                </div>
                <div className="text-xs text-slate-500 mt-1 truncate">{it.patient} • {it.reason} • {it.encounterId?.slice(0,8)}</div>
              </div>
              <div className="text-right shrink-0 ml-4">
                <div className="font-black text-red-600">₹{it.leakageAmount}</div>
                <div className="text-xs text-slate-500 max-w-[180px] truncate">{it.reason}</div>
              </div>
            </div>
          ))}
          {data.count===0 && <div className="p-8 text-center text-sm text-slate-500">No leaks — well billed!</div>}
        </div>
      </div>
    </div>
  );
}
