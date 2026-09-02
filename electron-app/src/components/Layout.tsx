import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../lib/auth';
import {
  LayoutDashboard, Users, CalendarDays, Mic, FileText, IndianRupee,
  ShieldCheck, Pill, FlaskConical, BedDouble, Settings, LogOut, Activity, Sparkles
} from 'lucide-react';

const items = [
  { label:'Command Center', to:'/', icon: LayoutDashboard, roles:['Admin','Management','Doctor','Billing','FrontDesk','Nurse','Pharmacy','LabTechnician'] },
  { label:'Patients', to:'/patients', icon: Users, roles:['Admin','Doctor','FrontDesk','Nurse'] },
  { label:'Appointments', to:'/appointments', icon: CalendarDays, roles:['Admin','FrontDesk','Doctor','Nurse'] },
  { label:'Medical Scribe', to:'/scribe', icon: Mic, roles:['Admin','Doctor'] },
  { label:'Discharge', to:'/discharge', icon: FileText, roles:['Admin','Doctor','Nurse'] },
  { label:'Revenue AI', to:'/revenue', icon: IndianRupee, roles:['Admin','Billing','Management'] },
  { label:'Claims', to:'/claims', icon: ShieldCheck, roles:['Admin','Billing','Management'] },
  { label:'Pharmacy', to:'/pharmacy', icon: Pill, roles:['Admin','Pharmacy','Management'] },
  { label:'Lab', to:'/lab', icon: FlaskConical, roles:['Admin','Doctor','LabTechnician','Nurse'] },
  { label:'Beds & Ops', to:'/beds', icon: BedDouble, roles:['Admin','Nurse','FrontDesk','Management'] },
  { label:'Admin', to:'/admin', icon: Settings, roles:['Admin'] },
];

export function Layout({ children }: { children:any }){
  const { user, logout } = useAuth();
  const loc = useLocation();
  const nav = useNavigate();
  const role = user?.role || 'FrontDesk';
  const visible = items;

  function doLogout(){ logout(); nav('/login'); }

  return (
    <div className="min-h-screen flex bg-[#f8fafc]">
      <aside className="w-[268px] shrink-0 bg-gradient-to-b from-slate-900 via-slate-900 to-slate-800 text-slate-100 flex flex-col shadow-xl">
        <div className="px-5 py-5 border-b border-white/10">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 rounded-xl bg-gradient-to-br from-blue-600 to-indigo-600 flex items-center justify-center shadow-glow">
              <Activity className="w-5 h-5 text-white" />
            </div>
            <div>
              <div className="text-[13px] font-extrabold tracking-[0.12em]">AI SMART HOSPITAL</div>
              <div className="text-[11px] text-slate-400 -mt-0.5">50-bed • Pune • HMIS AI Layer</div>
            </div>
          </div>
          <div className="mt-3 inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full bg-amber-500/15 text-amber-300 text-[10px] font-bold tracking-widest border border-amber-500/20">
            <Sparkles className="w-3 h-3" /> HUMAN APPROVAL REQUIRED
          </div>
        </div>
        <nav className="flex-1 overflow-y-auto py-3 px-3 space-y-1">
          {visible.map(it=>{
            const active = loc.pathname===it.to;
            const allowed = it.roles.includes(role);
            const Icon = it.icon as any;
            return (
              <Link key={it.to} to={it.to} className={`group flex items-center gap-3 px-3 py-2.5 rounded-xl text-[13px] font-medium transition-all ${active? 'bg-white text-slate-900 shadow-soft' : 'text-slate-300 hover:bg-white/10 hover:text-white'} ${!allowed?'opacity-60':''}`}>
                <span className={`w-8 h-8 rounded-lg flex items-center justify-center ${active? 'bg-slate-900 text-white' : 'bg-white/10 group-hover:bg-white/15'}`}>
                  <Icon className="w-[16px] h-[16px]" />
                </span>
                <span className="flex-1">{it.label}</span>
                {it.label==='Revenue AI' && <span className="text-[10px] font-bold bg-red-500 text-white px-1.5 py-0.5 rounded-full">ROI</span>}
                {!allowed && <span className="text-[10px] bg-white/10 px-1.5 py-0.5 rounded-full">view</span>}
              </Link>
            );
          })}
        </nav>
        <div className="p-4 border-t border-white/10 bg-white/[0.02]">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 rounded-full bg-gradient-to-br from-blue-500 to-indigo-500 flex items-center justify-center text-white font-bold text-sm">{user?.fullName?.[0]||'U'}</div>
            <div className="flex-1 min-w-0">
              <div className="text-sm font-semibold truncate">{user?.fullName}</div>
              <div className="text-xs text-slate-400 truncate">{user?.role} • {user?.userName}</div>
            </div>
          </div>
          <button onClick={doLogout} className="mt-3 w-full flex items-center justify-center gap-2 text-xs bg-white text-slate-900 hover:bg-slate-100 py-2 rounded-xl font-semibold transition"><LogOut className="w-3.5 h-3.5" /> Sign out</button>
          <div className="mt-3 flex items-center justify-between text-[11px] text-slate-500">
            <span>API {(import.meta as any).env?.VITE_API_URL || 'http://localhost:5115'}</span>
            <span className="flex items-center gap-1 text-emerald-400"><span className="w-2 h-2 bg-emerald-400 rounded-full animate-pulse" /> online</span>
          </div>
        </div>
      </aside>
      <div className="flex-1 flex flex-col min-w-0">
        <header className="h-[64px] bg-white/80 backdrop-blur-xl border-b border-slate-200/60 flex items-center justify-between px-6 sticky top-0 z-20">
          <div className="flex items-center gap-3 text-sm">
            <span className="hidden sm:inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-slate-900 text-white text-xs font-semibold"><span className="w-2 h-2 bg-emerald-400 rounded-full animate-pulse" /> Live</span>
            <span className="text-slate-600">
              {role==='Doctor' && 'Doctor queue • Scribe ready • AI drafts need sign'}
              {role==='FrontDesk' && 'Front desk • Appointments & registration • FAQ assistant'}
              {role==='Billing' && 'Finance • Revenue leakage & claims queue'}
              {role==='Management' && 'Executive • Command center • KPIs live'}
              {role==='Admin' && 'Administrator • All modules • Audit'}
              {role==='Pharmacy' && 'Pharmacy • Stock forecast & expiry risk'}
              {role==='Nurse' && 'Nursing • Beds & ward • Lab TAT'}
            </span>
          </div>
          <div className="flex items-center gap-3">
            <span className="hidden lg:inline text-xs text-slate-500 bg-slate-50 border px-3 py-1.5 rounded-full">AI recommends — humans approve. No autonomous diagnosis.</span>
            <span className="px-3 py-1.5 rounded-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white text-xs font-bold shadow-glow">{role}</span>
          </div>
        </header>
        <main className="flex-1 overflow-auto p-6 lg:p-7 bg-gradient-to-b from-[#f8fafc] to-[#f1f5f9]">{children}</main>
        <footer className="px-6 py-3 text-[11px] text-slate-500 bg-white/70 backdrop-blur border-t border-slate-200">All AI clinical outputs are <b>decision-support drafts</b> pending clinician sign-off • DPDP • ABDM FHIR-ready • Audit-logged • FHIR SNOMED/LOINC/ICD-10 ready</footer>
      </div>
    </div>
  );
}
