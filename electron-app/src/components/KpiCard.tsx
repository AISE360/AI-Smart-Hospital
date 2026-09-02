export function KpiCard({ label, value, unit, delta, sub, color }: { label:string; value:any; unit?:string; delta?:number; sub?:string; color?:'red'|'amber'|'green'|'blue'|'slate' }){
  const deltaColor = delta==null ? 'text-slate-400' : delta>0 ? (label.toLowerCase().includes('rejection')||label.toLowerCase().includes('stock') ? 'text-red-600' : 'text-green-600') : delta<0 ? (label.toLowerCase().includes('rejection') ? 'text-green-600' : 'text-red-600') : 'text-slate-400';
  const border = color==='red' ? 'border-red-200' : color==='amber' ? 'border-amber-200' : color==='green' ? 'border-green-200' : 'border-slate-200';
  return (
    <div className={`bg-white rounded-xl border ${border} p-4 shadow-sm`}>
      <div className="text-xs font-semibold tracking-wide text-slate-500 uppercase">{label}</div>
      <div className="mt-1 flex items-baseline gap-2">
        <span className="text-2xl font-bold text-slate-900">{value}</span>
        {unit && <span className="text-sm text-slate-500">{unit}</span>}
        {delta!=null && <span className={`text-xs font-semibold ${deltaColor}`}>{delta>0?'+':''}{delta.toFixed(1)}%</span>}
      </div>
      {sub && <div className="text-xs text-slate-400 mt-1">{sub}</div>}
    </div>
  );
}
