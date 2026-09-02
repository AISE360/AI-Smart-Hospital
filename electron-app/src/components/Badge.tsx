import { Sparkles, ShieldCheck, Circle } from 'lucide-react';
export function AiDraftBadge(){ return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-[11px] font-extrabold tracking-wide bg-amber-400 text-slate-900 border border-amber-500 shadow-sm"><Sparkles className="w-3 h-3" /> AI DRAFT — review required</span>; }
export function StatusBadge({status}:{status:string}){
  const s = status.toLowerCase();
  const isSigned = s.includes('signed')||s.includes('approved');
  const isDraft = s.includes('draft')||s.includes('ai');
  const cls = isSigned ? 'bg-emerald-500 text-white border-emerald-600 shadow-sm' : isDraft ? 'bg-amber-100 text-amber-900 border-amber-300' : 'bg-slate-100 text-slate-700 border-slate-200';
  const Icon = isSigned ? ShieldCheck : Circle;
  return <span className={`inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-bold border ${cls}`}><Icon className="w-3 h-3" />{status}</span>;
}
export function Pill({children, color='slate'}:{children:any, color?:'red'|'amber'|'green'|'slate'|'blue'}){
  const map:any={
    red:'bg-gradient-to-r from-red-500 to-orange-500 text-white border-transparent shadow-sm',
    amber:'bg-amber-100 text-amber-900 border-amber-200',
    green:'bg-emerald-500 text-white border-transparent shadow-sm',
    slate:'bg-slate-900 text-white border-transparent',
    blue:'bg-gradient-to-r from-blue-600 to-indigo-600 text-white border-transparent shadow-glow'
  };
  return <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-bold border ${map[color]}`}>{children}</span>;
}
