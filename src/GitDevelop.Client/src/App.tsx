import './App.css'

import HomePage from "./pages/HomePage.tsx";
import Login from "./pages/Login.tsx";
import {Routes, Route} from "react-router";

function App() {
  return (
    <Routes>
        <Route path="/" element={<HomePage/>}/>
        <Route path="/login" element={<Login/>}/>
    </Routes>
  )
}

export default App
