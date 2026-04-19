import './App.css'

import HomePage from "./pages/HomePage.tsx";
import Login from "./pages/Login.tsx";
import {Routes, Route} from "react-router";
import Workspace from "./pages/Workspace.tsx";

function App() {
  return (
    <Routes>
        <Route path="/" element={<HomePage/>}/>
        <Route path="/login" element={<Login/>}/>
        <Route path="/workspace" element={<Workspace/>}/>
    </Routes>
  )
}

export default App
