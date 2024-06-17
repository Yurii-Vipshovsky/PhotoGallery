import { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import './AlbumShow.css';

const AlbumShow = () => {
    const { albumId } = useParams();
    const [album, setAlbum] = useState(null);
    const [images, setImages] = useState([]);
    const [error, setError] = useState('');
    const [pageNumber, setPageNumber] = useState(1);
    const [totalImages, setTotalImages] = useState(0);
    const [pageSize] = useState(5);
    const user = useSelector((state) => state.user);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchAlbum = async () => {
            try {
                const response = await axios.get('/api/album/GetAlbum/' + albumId);
                setAlbum(response.data);
                setTotalImages(response.data.imageCount);
            } catch (error) {
                console.error('Error fetching album:', error);
            }
        };

        fetchAlbum();
    }, [albumId]);

    useEffect(() => {
        const fetchImages = async () => {
            try {
                const response = await axios.get(`/api/album/GetImages/${albumId}`, {
                    params: { pageNumber, pageSize }
                });
                setImages(response.data);
            } catch (error) {
                console.error('Error fetching images:', error);
            }
        };

        fetchImages();
    }, [albumId, pageNumber, pageSize]);

    const handleNextPage = () => {
        if ((pageNumber * pageSize) < totalImages) {
            setPageNumber(pageNumber + 1);
        }
    };

    const handlePreviousPage = () => {
        if (pageNumber > 1) {
            setPageNumber(pageNumber - 1);
        }
    };
    const handleLike = async (imageId) => {
        try {
            await axios.post(
                `/api/image/Like/${imageId}`,
                {
                    headers: { Authorization: `Bearer ${user.token}` }
                }
            );
            setImages(images.map(img => img.id === imageId ? { ...img, likes: img.likes + 1 } : img));
        } catch (error) {
            setError(error.message);
        }
    };

    const handleDislike = async (imageId) => {
        try {
            await axios.post(
                `/api/image/Dislike/${imageId}`,
                {
                    headers: { Authorization: `Bearer ${user.token}` }
                }
            );
            setImages(images.map(img => img.id === imageId ? { ...img, dislikes: img.dislikes + 1 } : img));
        } catch (error) {
            setError(error.message);
        }
    };

    const handleDeleteImage = async (imageId) => {
        try {
            await axios.delete(`/api/image/Delete/${imageId}`, {
                headers: { Authorization: `Bearer ${user.token}` }
            });
            setTotalImages(totalImages - 1);
            setImages(images.filter(img => img.id !== imageId));
        } catch (error) {
            setError(error.message);
        }
    };

    if (!album) {
        return <div>Loading...</div>;
    }

    return (
        <div>
            {error && <p className="error">{error}</p>}
            <h1>{album.name}</h1>
            <p>{album.description}</p>
            {user.userName === album.userId && (
                <button className="add-image" onClick={() => { navigate(`/image-add/${albumId}`) }}>Add Image</button>
            )}
            <div>
                {images!=null && images.map((image) => (
                    <div key={image.id}>
                        <img
                            className="thumbnail"
                            src={image.imagePath}
                            alt={image.imageName}
                            onClick={() => window.open(image.imagePath, '_blank')}
                        />
                        <p>Likes: {image.likes}</p>
                        <p>Dislikes: {image.dislikes}</p>
                        {user.isAuthenticated && (
                            <div className="buttons-conteiner">
                                <button onClick={() => handleLike(image.id)}>Like</button>
                                <button onClick={() => handleDislike(image.id)}>Dislike</button>
                                {(user.isAdmin || user.userName === album.userId) && (
                                    <button onClick={() => handleDeleteImage(image.id)}>Delete</button>
                                )}
                            </div>
                        )}
                    </div>
                ))}
            </div>
            <div className="buttons-conteiner">
                <button onClick={handlePreviousPage} disabled={pageNumber === 1}>Previous</button>
                <button onClick={handleNextPage} disabled={(pageNumber * pageSize) >= totalImages}>Next</button>
            </div>
        </div>
    );
};

export default AlbumShow;

