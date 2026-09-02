import { useEffect, useState } from 'react';
import { api } from '../lib/api';

export default function Pharmacy(){
  const [items,setItems]=useState<any[]>([]);
  const [expiry,setExpiry]=useState<any[]>([]);
  const [stockout,setStockout]=useState<any[]>([]);
  const [forecast,setForecast]=useState<any>(null);

  async function load(){
    const [i,e,s]=await Promise.all([api.get('/pharmacy/items'), api.get('/pharmacy/expiry-alerts'), api.get('/pharmacy/stockout-prediction')]);
    setItems(i.data); setExpiry(e.data); setStockout(s.data);
  }
  useEffect(()=>{ load(); },[]);

  async function doForecast(id:string){
    const r=await api.post(`/pharmacy/forecast/${id}`, {history:[12,14,10,15,13,16,12]});
    setForecast(r.data);
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Pharmacy AI <span className="text-xs font-normal text-slate-500">Demand forecast • Expiry risk • Stock-out prediction (moving-average, swappable ML)</span></h1>
      <div className="grid md:grid-cols-3 gap-4">
        <div className="bg-white rounded-xl border p-4"><div className="text-sm font-semibold text-red-600">Expiry Risk (90d)</div><div className="text-2xl font-bold">{expiry.length} batches</div><div className="text-xs text-slate-500">Value at risk ₹{expiry.reduce((a:any,c:any)=>a+c.valueAtRisk,0)}</div></div>
        <div className="bg-white rounded-xl border p-4"><div className="text-sm font-semibold text-amber-600">Stock-out &lt;14 days</div><div className="text-2xl font-bold">{stockout.length} items</div><div className="text-xs text-slate-500">Reorder before stock-out</div></div>
        <div className="bg-white rounded-xl border p-4"><div className="text-sm font-semibold">Forecast Method</div><div className="text-sm font-mono">moving_average</div><div className="text-xs text-slate-500">Pluggable for real ML model later</div></div>
      </div>

      <div className="grid md:grid-cols-2 gap-6">
        <div className="bg-white rounded-xl border">
          <div className="p-4 border-b text-sm font-semibold">Stock & Forecast</div>
          <div className="divide-y max-h-96 overflow-auto">
            {items.map((it:any)=>(
              <div key={it.id} className="p-3 flex items-center justify-between">
                <div><div className="text-sm font-medium">{it.name} <span className="text-xs border px-1 rounded">{it.code}</span></div><div className="text-xs text-slate-500">{it.stock?.quantityOnHand} units • {it.stock?.daysOfStock ?? '—'} days left • {it.expiryRisk} expiry risks</div></div>
                <button onClick={()=>doForecast(it.id)} className="text-xs border px-2 py-1 rounded">Forecast</button>
              </div>
            ))}
          </div>
        </div>
        <div className="space-y-4">
          <div className="bg-white rounded-xl border p-4">
            <div className="text-sm font-semibold">Expiry Alerts</div>
            <div className="mt-2 space-y-1">
              {expiry.slice(0,6).map((e:any)=><div key={e.id} className="flex justify-between text-xs border rounded px-2 py-1 bg-red-50"><span>{e.itemName} {e.batchNumber}</span><span>{e.daysToExpiry}d • ₹{e.valueAtRisk}</span></div>)}
            </div>
          </div>
          {forecast && <div className="bg-blue-50 border border-blue-200 rounded-xl p-4"><div className="text-sm font-semibold">Forecast Result</div><pre className="text-xs mt-2">{JSON.stringify(forecast,null,2)}</pre></div>}
        </div>
      </div>
    </div>
  );
}
