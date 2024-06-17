import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useSelector } from 'react-redux';
import axios from 'axios';

const ImageCreate = () => {
    const { albumId } = useParams();
    const [selectedFile, setSelectedFile] = useState(null);
    const [uploadSuccess, setUploadSuccess] = useState(false);
    const [uploadError, setUploadError] = useState('');
    const token = useSelector((state) => state.user.token);

    const handleFileChange = (event) => {
        setSelectedFile(event.target.files[0]);
    };

    const handleUpload = async () => {
        if (!selectedFile) {
            setUploadError('Please select a file first.');
            return;
        }

        const formData = new FormData();
        formData.append('image', selectedFile);
        formData.append('albumId', albumId);

        try {
            const response = await axios.post('/api/image/AddImageToAlbum', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Authorization: `Bearer ${token}` 
                }
            });

            if (response.status === 200) {
                setUploadSuccess(true);
                setUploadError('');
            } else {
                setUploadSuccess(false);
                setUploadError('Failed to upload the image.');
            }
        } catch (error) {
            console.error('Error uploading the image:', error);
            setUploadSuccess(false);
            setUploadError('Failed to upload the image.');
        }
    };

    return (
        <div>
            <h2>Upload Image</h2>
            <input type="file" onChange={handleFileChange} />
            <button onClick={handleUpload}>Upload</button>
            {uploadSuccess && <p>Image uploaded successfully!</p>}
            {uploadError && <p className="error">{uploadError}</p>}
        </div>
    );
};

export default ImageCreate;
