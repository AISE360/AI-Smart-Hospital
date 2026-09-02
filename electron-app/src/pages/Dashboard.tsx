import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { KpiCard } from '../components/KpiCard';
import * as signalR from '@microsoft/signalr';
import { BedDouble, Users, ArrowLeftRight, Banknote, AlertTriangle, ShieldAlert, PackageX, RotateCcw, Sparkles, Radio, Clock } from 'lucide-react';

export default function Dashboard(){
  const [data,setData]=useState<any>(null);
  const [insight,setInsight]=useState<string>('');
  const [loading,setLoading]=useState(true);
  const [live,setLive]=useState(false);

  async function load(){
    const [k,r]=await Promise.all([api.get('/dashboard/kpis'), api.get('/dashboard/insight').catch(()=>({data:{insight:'Insight unavailable offline'}}))]);
    setData(k.data); setInsight(r.data.insight); setLoading(false);
  }
  useEffect(()=>{ load(); },[]);

  useEffect(()=>{
    const token = localStorage.getItem('token');
    const conn = new signalR.HubConnectionBuilder()
      .withUrl(((import.meta as any).env?.VITE_API_URL||'http://localhost:5115')+'/hubs/dashboard', { accessTokenFactory:()=> token||'' })
      .withAutomaticReconnect()
      .build();
    conn.on('KpiUpdated', ()=>{ setLive(true); setTimeout(()=>setLive(false),3000); load(); });
    conn.start().then(()=>conn.invoke('SubscribeToKpis')).catch(()=>{});
    return ()=>{ conn.stop(); };
  },[]);

  if(loading) return (
    <div className="flex items-center justify-center h-64">
      <div className="flex flex-col items-center gap-3">
        <div className="w-10 h-10 rounded-2xl bg-gradient-to-br from-blue-600 to-indigo-600 animate-pulse" />
        <div className="text-sm font-semibold text-slate-600">Loading command center…</div>
      </div>
    </div>
  );
  const d=data;
  return (
    <div className="space-y-6 animate-fade-in">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-blue-600 text-white text-xs font-bold shadow-glow"><Sparkles className="w-3.5 h-3.5" /> COMMAND CENTER • LIVE</div>
          <h1 className="text-[28px] font-extrabold tracking-tight mt-2">Hospital Command Center</h1>
          <p className="text-sm text-slate-500 -mt-1">Live KPIs via SignalR • AI Insight answers “What changed? Why? What needs attention?”</p>
        </div>
        <div className="flex items-center gap-2">
          {live && <span className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-emerald-500 text-white text-xs font-bold rounded-full animate-pulse shadow"><Radio className="w-3 h-3" /> LIVE UPDATE</span>}
          <span className="inline-flex items-center gap-1.5 text-xs text-slate-500 bg-white border px-3 py-1.5 rounded-full"><Clock className="w-3 h-3" /> {new Date(d.generatedAt).toLocaleString('en-IN')}</span>
        </div>
      </div>

      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <KpiCard label="Beds Occupied" value={`${d.bedsOccupied}/${d.bedsTotal}`} sub={`${d.occupancyPct}% occupancy • 70–85% target`} delta={2.1} color="blue" icon={<BedDouble className="w-5 h-5" />} />
        <KpiCard label="OPD Today" value={d.opdToday} unit="visits" sub="120–160 target • 10–15 depts" delta={5.4} icon={<Users className="w-5 h-5" />} />
        <KpiCard label="Admissions / Discharges" value={`${d.admissionsToday} / ${d.dischargesToday}`} sub={`Avg LOS ${d.avgLos} days • target 3.2`} icon={<ArrowLeftRight className="w-5 h-5" />} />
        <KpiCard label="Revenue Today" value={`₹${(d.revenueToday/100000).toFixed(1)}L`} unit="INR" sub="₹1.8cr/mo • 45% insurance" delta={-1.2} color="green" icon={<Banknote className="w-5 h-5" />} />
      </div>
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <KpiCard label="Recoverable Leakage" value={`₹${(d.leakageAlerts* 8500/1000).toFixed(0)}K`} sub={`${d.leakageAlerts} alerts • ₹1–2L/mo flagged`} color="red" icon={<AlertTriangle className="w-5 h-5" />} />
        <KpiCard label="Outstanding Claims" value={`₹${(d.outstandingClaims/100000).toFixed(1)}L`} sub={`${d.rejectionRate}% rejection • target <10%`} delta={d.rejectionRate-12} color={d.rejectionRate>12?'red':'green'} icon={<ShieldAlert className="w-5 h-5" />} />
        <KpiCard label="Expiry Alerts" value={d.expiryAlerts} sub="90-day window • pharmacy" color={d.expiryAlerts>2?'amber':'slate'} icon={<PackageX className="w-5 h-5" />} />
        <KpiCard label="Readmissions" value="2" unit="this week" sub="target <5% • quality KPI" icon={<RotateCcw className="w-5 h-5" />} />
      </div>

      <div className="relative overflow-hidden rounded-[20px] bg-gradient-to-br from-slate-900 via-slate-900 to-indigo-900 p-[1px] shadow-card">
        <div className="rounded-[19px] bg-gradient-to-br from-slate-900 via-slate-900 to-indigo-950 p-6 text-white relative overflow-hidden">
          <div className="absolute -top-20 -right-20 w-80 h-80 bg-blue-600/20 rounded-full blur-3xl" />
          <div className="absolute -bottom-20 -left-20 w-80 h-80 bg-indigo-600/15 rounded-full blur-3xl" />
          <div className="relative">
            <div className="flex items-center gap-2 text-xs font-bold tracking-[0.15em] text-blue-300"><Sparkles className="w-4 h-4" /> AI INSIGHT PANEL — DAILY SUMMARY</div>
            <div className="mt-3 text-[15px] leading-relaxed whitespace-pre-wrap font-medium">{insight}</div>
            <div className="mt-4 flex flex-wrap items-center gap-2 text-xs">
              <span className="px-2.5 py-1 rounded-full bg-white/10 border border-white/15">IAiClient • aggregated deltas • no PHI</span>
              <span className="px-2.5 py-1 rounded-full bg-white/10 border border-white/15">stub-model v1</span>
              <span className="text-white/60">Human review not required for aggregate insight</span>
            </div>
          </div>
        </div>
      </div>

      <div className="bg-white rounded-2xl border border-slate-200/70 shadow-soft p-5">
        <div className="flex items-center justify-between"><div className="text-sm font-extrabold tracking-tight">KPI Snapshots</div><span className="text-xs bg-slate-900 text-white px-2 py-1 rounded-full">{d.kpis?.length||0} metrics</span></div>
        <div className="mt-4 grid md:grid-cols-2 gap-3">
          {(d.kpis||[]).map((k:any)=>(
            <div key={k.metricName} className="group flex items-center justify-between border border-slate-200 rounded-xl px-4 py-3 hover:border-blue-200 hover:bg-blue-50/50 transition">
              <div><div className="text-sm font-semibold">{k.metricName}</div><div className="text-xs text-slate-500 capitalize">{k.category}</div></div>
              <div className="text-right"><div className="font-extrabold">{k.value}{k.unit==='%'?'%':k.unit==='INR'?'':' '+k.unit}</div>{k.deltaPercent!=null && <span className={`text-xs font-bold px-1.5 py-0.5 rounded-full ${k.deltaPercent>0?'bg-emerald-50 text-emerald-700 border border-emerald-200':'bg-red-50 text-red-700 border border-red-200'}`}>{k.deltaPercent>0?'+':''}{k.deltaPercent.toFixed(1)}%</span>}</div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
