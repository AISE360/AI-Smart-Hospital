import { useEffect, useState } from 'react';
import { api } from '../lib/api';

export default function Lab(){
  const [orders,setOrders]=useState<any[]>([]);
  const [tat,setTat]=useState<any>(null);
  const [critical,setCritical]=useState<any[]>([]);

  async function load(){
    const [o,t,c]=await Promise.all([api.get('/lab/orders'), api.get('/lab/tat'), api.get('/lab/critical')]);
    setOrders(o.data); setTat(t.data); setCritical(c.data);
  }
  useEffect(()=>{ load(); },[]);

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Lab & Turnaround</h1>
      <div className="grid md:grid-cols-3 gap-4">
        <div className="bg-white rounded-xl border p-4"><div className="text-xs font-semibold">Avg TAT</div><div className="text-2xl font-bold">{tat?.avgTurnaroundHours} hrs</div><div className="text-xs text-slate-500">{tat?.totalOrders} orders</div></div>
        <div className="bg-white rounded-xl border p-4"><div className="text-xs font-semibold text-red-600">Critical Pending Routing</div><div className="text-2xl font-bold">{critical.length}</div></div>
        <div className="bg-white rounded-xl border p-4 text-xs">{tat?.byPriority?.map((p:any)=><div key={p.priority} className="flex justify-between"><span>{p.priority}</span><span>{p.avgHours}h ({p.count})</span></div>)}</div>
      </div>
      <div className="bg-white rounded-xl border">
        <div className="p-4 border-b text-sm font-semibold">Orders ({orders.length})</div>
        <div className="divide-y max-h-96 overflow-auto">
          {orders.map((o:any)=><div key={o.id} className="p-3 flex items-center justify-between text-sm"><span>{o.testName} ({o.testCode}) • {o.patient} • {o.status} {o.isCritical && <span className="ml-2 bg-red-600 text-white text-xs px-1.5 py-0.5 rounded">CRITICAL</span>}</span><span className="text-xs text-slate-500">{o.result || 'pending'} • {o.priority}</span></div>)}
        </div>
      </div>
    </div>
  );
}
