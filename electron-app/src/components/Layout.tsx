import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../lib/auth';



const items = [
  { label:'Command Center', to:'/', icon:'◉', roles:['Admin','Management','Doctor','Billing','FrontDesk','Nurse','Pharmacy','LabTechnician'] },
  { label:'Patients', to:'/patients', icon:'👤', roles:['Admin','Doctor','FrontDesk','Nurse'] },
  { label:'Appointments', to:'/appointments', icon:'📅', roles:['Admin','FrontDesk','Doctor','Nurse'] },
  { label:'Medical Scribe', to:'/scribe', icon:'🎙️', roles:['Admin','Doctor'] },
  { label:'Discharge', to:'/discharge', icon:'📝', roles:['Admin','Doctor','Nurse'] },
  { label:'Revenue AI', to:'/revenue', icon:'₹', roles:['Admin','Billing','Management'] },
  { label:'Claims', to:'/claims', icon:'🛡️', roles:['Admin','Billing','Management'] },
  { label:'Pharmacy', to:'/pharmacy', icon:'💊', roles:['Admin','Pharmacy','Management'] },
  { label:'Lab', to:'/lab', icon:'🧪', roles:['Admin','Doctor','LabTechnician','Nurse'] },
  { label:'Beds & Ops', to:'/beds', icon:'🛏️', roles:['Admin','Nurse','FrontDesk','Management'] },
  { label:'Admin', to:'/admin', icon:'⚙️', roles:['Admin'] },
];

export function Layout({ children }: { children:any }){
  const { user, logout } = useAuth();
  const loc = useLocation();
  const nav = useNavigate();
  const role = user?.role || 'FrontDesk';

  // Show all for admin, filtered for others - for demo show all with dim
  const visible = role==='Admin' ? items : items; // keep all visible for demo but highlight role home

  function doLogout(){ logout(); nav('/login'); }

  return (
    <div className="min-h-screen flex">
      <aside className="w-64 bg-slate-900 text-slate-100 flex flex-col">
        <div className="px-5 py-4 border-b border-slate-800">
          <div className="text-sm font-bold tracking-wide">AI SMART HOSPITAL</div>
          <div className="text-xs text-slate-400">50-bed • Pune • HMIS AI Layer</div>
          <div className="mt-2 inline-flex px-2 py-0.5 rounded bg-amber-500/20 text-amber-300 text-[10px] font-bold tracking-widest">HUMAN APPROVAL REQUIRED</div>
        </div>
        <nav className="flex-1 overflow-y-auto py-2">
          {visible.map(it=>{
            const active = loc.pathname===it.to;
            const allowed = it.roles.includes(role);
            return (
              <Link key={it.to} to={it.to} className={`flex items-center gap-3 px-4 py-2.5 text-sm ${active? 'bg-slate-800 text-white border-l-2 border-blue-500' : 'text-slate-300 hover:bg-slate-800/60 hover:text-white'} ${!allowed?'opacity-50':''}`}>
                <span className="w-6 text-center">{it.icon}</span>{it.label}
                {!allowed && <span className="ml-auto text-[10px] bg-slate-700 px-1.5 py-0.5 rounded">view</span>}
              </Link>
            );
          })}
        </nav>
        <div className="p-4 border-t border-slate-800">
          <div className="text-sm font-medium">{user?.fullName}</div>
          <div className="text-xs text-slate-400">{user?.role} • {user?.userName}</div>
          <button onClick={doLogout} className="mt-3 w-full text-xs bg-slate-800 hover:bg-slate-700 py-2 rounded">Sign out</button>
          <div className="mt-3 text-[11px] text-slate-500">API: {(import.meta as any).env?.VITE_API_URL || 'http://localhost:5000'} • <span className="text-green-400">● online</span></div>
        </div>
      </aside>
      <div className="flex-1 flex flex-col bg-slate-50">
        <header className="h-14 bg-white border-b flex items-center justify-between px-6">
          <div className="text-sm text-slate-600">
            {role==='Doctor' && 'Doctor queue • Scribe ready'}
            {role==='FrontDesk' && 'Front desk • Appointments & registration'}
            {role==='Billing' && 'Finance • Revenue & claims queue'}
            {role==='Management' && 'Executive • Command center'}
            {role==='Admin' && 'Administrator • All modules'}
            {role==='Pharmacy' && 'Pharmacy • Stock & expiry'}
            {role==='Nurse' && 'Nursing • Beds & ward'}
          </div>
          <div className="flex items-center gap-3">
            <span className="text-xs text-slate-500 hidden md:inline">AI recommends — humans approve. No autonomous diagnosis.</span>
            <span className="px-2.5 py-1 rounded-full bg-blue-600 text-white text-xs font-semibold">{role}</span>
          </div>
        </header>
        <main className="flex-1 overflow-auto p-6">{children}</main>
        <footer className="px-6 py-2 text-[11px] text-slate-400 bg-white border-t">All AI clinical outputs are decision-support drafts pending clinician sign-off • DPDP • ABDM FHIR-ready • Audit-logged</footer>
      </div>
    </div>
  );
}
