import { useState } from 'react';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';

const AlbumCreate = () => {
    const [albumName, setAlbumName] = useState('');
    const [description, setDescription] = useState('');
    const [error, setError] = useState('');
    const token = useSelector((state) => state.user.token);
    const navigate = useNavigate();

    const handleCreateAlbum = async (event) => {
        event.preventDefault();

        try {
            await axios.post(
                'api/album/Create',
                { albumName, description },
                {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    }
                }
            );
            navigate('/');
        } catch (error) {
            setError(error.message);
        }
    };

    return (
        <div>
            <h2>Create Album</h2>
            {error && <p className="error">{error}</p>}
            <form onSubmit={handleCreateAlbum}>
                <div>
                    <label htmlFor="albumName">Name</label>
                    <input
                        type="text"
                        name="albumName"
                        value={albumName}
                        onChange={(e) => setAlbumName(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="description">Description</label>
                    <input
                        type="text"
                        name="description"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Create Album</button>
            </form>
        </div>
    );
};

export default AlbumCreate;
