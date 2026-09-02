import { TrendingUp, TrendingDown, Minus } from 'lucide-react';

export function KpiCard({ label, value, unit, delta, sub, color, icon }: { label:string; value:any; unit?:string; delta?:number; sub?:string; color?:'red'|'amber'|'green'|'blue'|'slate', icon?: React.ReactNode }){
  const isPositiveGood = !(label.toLowerCase().includes('rejection')||label.toLowerCase().includes('stock')||label.toLowerCase().includes('expiry'));
  let deltaColor = 'text-slate-400';
  let TrendIcon = Minus;
  if(delta!=null){
    if(delta>0){ deltaColor = isPositiveGood ? 'text-emerald-600 bg-emerald-50 border-emerald-200' : 'text-red-600 bg-red-50 border-red-200'; TrendIcon = TrendingUp; }
    else if(delta<0){ deltaColor = isPositiveGood ? 'text-red-600 bg-red-50 border-red-200' : 'text-emerald-600 bg-emerald-50 border-emerald-200'; TrendIcon = TrendingDown; }
  }
  const accent = color==='red' ? 'from-red-500 to-orange-500' : color==='amber' ? 'from-amber-500 to-orange-500' : color==='green' ? 'from-emerald-500 to-teal-500' : color==='blue' ? 'from-blue-600 to-indigo-600' : 'from-slate-700 to-slate-900';
  return (
    <div className="group bg-white rounded-2xl border border-slate-200/70 p-[1px] shadow-soft hover:shadow-card hover:-translate-y-0.5 transition-all duration-200">
      <div className="bg-white rounded-[15px] p-4">
        <div className="flex items-start justify-between">
          <div className={`w-9 h-9 rounded-xl bg-gradient-to-br ${accent} flex items-center justify-center text-white shadow-sm`}>
            {icon || <span className="text-sm font-bold">{label[0]}</span>}
          </div>
          {delta!=null && (
            <span className={`inline-flex items-center gap-1 text-[11px] font-bold px-2 py-1 rounded-full border ${deltaColor}`}>
              <TrendIcon className="w-3 h-3" /> {delta>0?'+':''}{delta.toFixed(1)}%
            </span>
          )}
        </div>
        <div className="mt-3 text-[11px] font-bold tracking-[0.08em] text-slate-500 uppercase">{label}</div>
        <div className="mt-1 flex items-baseline gap-2">
          <span className="text-[26px] font-extrabold tracking-tight text-slate-900">{value}</span>
          {unit && <span className="text-sm font-medium text-slate-500">{unit}</span>}
        </div>
        {sub && <div className="text-xs text-slate-500 mt-1 leading-relaxed">{sub}</div>}
      </div>
    </div>
  );
}
