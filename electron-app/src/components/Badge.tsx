export function AiDraftBadge(){ return <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-semibold bg-amber-100 text-amber-800 border border-amber-300">AI DRAFT — review required</span>; }
export function StatusBadge({status}:{status:string}){
  const color = status.toLowerCase().includes('signed')||status.toLowerCase().includes('approved') ? 'bg-green-100 text-green-800 border-green-300' : status.toLowerCase().includes('draft')||status.toLowerCase().includes('ai') ? 'bg-amber-100 text-amber-800 border-amber-300' : 'bg-slate-100 text-slate-700 border-slate-300';
  return <span className={`inline-flex px-2 py-0.5 rounded text-xs font-medium border ${color}`}>{status}</span>;
}
export function Pill({children, color='slate'}:{children:any, color?:'red'|'amber'|'green'|'slate'|'blue'}){
  const map:any={red:'bg-red-100 text-red-700 border-red-300', amber:'bg-amber-100 text-amber-800 border-amber-300', green:'bg-green-100 text-green-700 border-green-300', slate:'bg-slate-100 text-slate-600 border-slate-300', blue:'bg-blue-100 text-blue-700 border-blue-300'};
  return <span className={`inline-flex px-2.5 py-1 rounded-full text-xs font-semibold border ${map[color]}`}>{children}</span>;
}
