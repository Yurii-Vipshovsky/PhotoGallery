import { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { Link, useParams } from 'react-router-dom';
import axios from 'axios';
import './AlbumList.css';

const AlbumList = () => {
    const { userId } = useParams();
    const [albums, setAlbums] = useState([]);
    const [error, setError] = useState('');
    const user = useSelector((state) => state.user);

    useEffect(() => {
        if (userId) {
            axios.get(`/api/album/GetUserAlbums/${userId}`,
                {
                    headers: {
                        'Authorization': `Bearer ${user.token}`
                    }
                }
            ).then((response) => {
                setAlbums(response.data);
            });
        }
        else {
            axios.get('/api/album/GetAlbums').then((response) => {
                setAlbums(response.data);
            });
        }
    }, [userId]);

    const handleDelete = async (albumId) => {
        try {
            await axios.delete('api/album/Delete/' + albumId,
                {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${user.token}`
                }
            })
            setAlbums(albums.filter(item => item.id !== albumId))
        }
        catch (error) {
            setError(error);
            console.error('There was an error while deleting album!', error);
        }
    }

    return (
        <div>
            {error && <p className="error">{error}</p>}
            {user.isAuthenticated &&
                <Link to="/album-create">
                    <button className="new-button">New Album</button>
                </Link>
            }
            <table className="album-table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Preview</th>
                        {(user.isAdmin || userId) &&
                            <th>Delete Album</th>
                        }
                    </tr>
                </thead>
                <tbody>
                    {albums.map((album) => (
                        <tr key={album.id}>
                            <td>
                                <Link to={`/album/${album.id}` }>
                                    {album.name}
                                </Link>
                            </td>
                            <td>{album.description}</td>
                            <td>
                                {album.images != null && album.images.length > 0 ? (
                                    <img src={album.images[0].imagePath} alt={album.name} />
                                ) : (
                                    <p>No images</p>
                                )}
                            </td>
                            {(user.isAdmin || userId) &&
                                <td><button onClick={() => { handleDelete(album.id) }}>Delete</button></td>
                            }
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default AlbumList;
