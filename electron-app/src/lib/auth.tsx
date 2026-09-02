import React, { createContext, useContext, useEffect, useState } from 'react';
import { api } from './api';

type User = { id:string; userName:string; fullName:string; role:string; roles:string[]; email:string };

type AuthCtx = {
  user: User | null;
  token: string | null;
  login: (username:string, password:string, mfa?:string)=>Promise<void>;
  logout: ()=>void;
  loading: boolean;
};

const Ctx = createContext<AuthCtx>(null as any);

export function AuthProvider({children}:{children:React.ReactNode}){
  const [user,setUser]=useState<User|null>(null);
  const [token,setToken]=useState<string|null>(()=>localStorage.getItem('token'));
  const [loading,setLoading]=useState(true);

  useEffect(()=>{
    if(!token){ setLoading(false); return; }
    api.get('/auth/me').then(r=>{
      const u=r.data;
      setUser({ id:u.id, userName:u.userName, fullName:u.fullName, role:u.role, roles:u.roles, email:u.email });
    }).catch(()=>{ localStorage.removeItem('token'); setToken(null); }).finally(()=>setLoading(false));
  },[token]);

  async function login(username:string,password:string,mfa?:string){
    const res = await api.post('/auth/login', { username, password, mfaCode: mfa });
    const t=res.data.token;
    localStorage.setItem('token', t);
    setToken(t);
    const me = await api.get('/auth/me', { headers:{Authorization:`Bearer ${t}`} });
    const u=me.data;
    setUser({ id:u.id, userName:u.userName, fullName:u.fullName, role:u.role, roles:u.roles, email:u.email});
  }
  function logout(){ localStorage.removeItem('token'); setUser(null); setToken(null); }

  return <Ctx.Provider value={{user,token,login,logout,loading}}>{children}</Ctx.Provider>;
}
export const useAuth = ()=> useContext(Ctx);
