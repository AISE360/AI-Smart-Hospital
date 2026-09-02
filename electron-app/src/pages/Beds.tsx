import { useEffect, useState } from 'react';
import { api } from '../lib/api';

export default function Beds(){
  const [data,setData]=useState<any>(null);
  const [beds,setBeds]=useState<any[]>([]);

  async function load(){ const o=await api.get('/beds/occupancy'); setData(o.data); const b=await api.get('/beds'); setBeds(b.data); }
  useEffect(()=>{ load(); },[]);

  if(!data) return <div>Loading beds…</div>;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Bed & Operations</h1>
      <div className="grid md:grid-cols-4 gap-4">
        <div className="bg-white rounded-xl border p-4"><div className="text-xs font-semibold">Occupancy</div><div className="text-2xl font-bold">{data.occupancyPct}%</div><div className="text-xs text-slate-500">{data.occupied}/{data.total}</div></div>
        <div className="bg-white rounded-xl border p-4"><div className="text-xs">Available</div><div className="text-2xl font-bold text-green-600">{data.available}</div></div>
        <div className="bg-white rounded-xl border p-4"><div className="text-xs">Expected Discharge 24h</div><div className="text-2xl font-bold">{data.forecast.expectedDischarges24h}</div></div>
        <div className="bg-white rounded-xl border p-4"><div className="text-xs">48h</div><div className="text-2xl font-bold">{data.forecast.expectedDischarges48h}</div></div>
      </div>
      <div className="bg-white rounded-xl border p-4">
        <div className="text-sm font-semibold">By Ward</div>
        <div className="mt-2 grid md:grid-cols-4 gap-2">
          {data.wards.map((w:any)=><div key={w.ward} className="border rounded p-2"><div className="font-medium text-sm">{w.ward}</div><div className="text-xs">{w.occupied}/{w.total} • {w.occupancyPct}%</div><div className="mt-1 h-2 bg-slate-100 rounded"><div className="h-2 bg-blue-600 rounded" style={{width:w.occupancyPct+'%'}}/></div></div>)}
        </div>
      </div>
      <div className="bg-white rounded-xl border">
        <div className="p-4 border-b text-sm font-semibold">Beds ({beds.length})</div>
        <div className="grid grid-cols-6 gap-2 p-4">
          {beds.map((b:any)=>(
            <div key={b.id} className={`p-2 rounded border text-xs text-center ${b.status==='Occupied'?'bg-red-50 border-red-200 text-red-800': b.status==='Available'?'bg-green-50 border-green-200 text-green-700':'bg-amber-50 border-amber-200'}`}>
              <div className="font-bold">{b.bedNumber}</div><div>{b.status}</div><div className="text-[10px]">{b.wardCode}</div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
