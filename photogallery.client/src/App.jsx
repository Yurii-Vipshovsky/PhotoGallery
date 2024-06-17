import { Route, Routes } from 'react-router-dom';
import './App.css';
import Header from './Components/Header/Header';
import Register from './Components/AutorizationComponents/Register';
import Login from './Components/AutorizationComponents/Login';
import AlbumList from './Components/AlbumComponents/AlbumList/AlbumList';
import AlbumCreate from './Components/AlbumComponents/AlbumCreate';
import AlbumShow from './Components/AlbumComponents/AlbumShow/AlbumShow';
import ImageCreate from './Components/AlbumComponents/ImageCreate';

const App = () => {
    return (
        <div className="app">
            <Header />
            <main className="main">
                <Routes>
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/" element={<AlbumList />} />
                    <Route path="/:userId" element={<AlbumList />} />
                    <Route path="/album-create" element={<AlbumCreate />} />
                    <Route path="/album/:albumId" element={<AlbumShow />} />
                    <Route path="/image-add/:albumId" element={<ImageCreate />} />
                </Routes>
            </main>
        </div>
    );
};

export default App;
