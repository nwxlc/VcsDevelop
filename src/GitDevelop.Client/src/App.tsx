import './App.css'
import Header from "./components/GlobalComponents/Header.tsx";
import HomePageBody from "./components/HomePage/HomePageBody.tsx";
function App() {
  return (
      <div className="container">
          <Header/>
          <HomePageBody/>     
      </div>
  )
}

export default App
