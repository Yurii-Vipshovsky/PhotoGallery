import { useState } from 'react';
import { login } from '../../userSlice';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post('/api/Account/login', {
                username,
                password,
            });
            dispatch(login({
                userName: username,
                isAdmin: response.data.isAdmin,
                token: response.data.token
            }));
            navigate('/');
        } catch (error) {
            setError(error);
        }
    };

    return (
        <div>
            {error && <p className="error">{error}</p>}
            <h2>Login</h2>
            <form onSubmit={handleLogin}>
                <div>
                    <label htmlFor="username">Username:</label>
                    <input type="text" name="username" value={username} onChange={(e) => setUsername(e.target.value)} />
                </div>
                <div>
                    <label htmlFor="password">Password:</label>
                    <input type="password" name="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                </div>
                <button type="submit">Login</button>
            </form>
        </div>
    );
};

export default Login;
