import { useState } from 'react';
import axios from 'axios';

const Register = () => {
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');


    const handleRegister = async (e) => {
        e.preventDefault();
        try {
            await axios.post('api/Account/register ', {
                username,
                email,
                password,
            });
        }
        catch (error) {
            setError(error);
        }   
    };

    return (
        <div>
            {error && <p className="error">{error}</p>}
            <h2>Register</h2>
            <form onSubmit={handleRegister}>
                <div>
                    <label htmlFor="username">Username:</label>
                    <input type="text" name="username" value={username} onChange={(e) => setUsername(e.target.value)} />
                </div>
                <div>
                    <label htmlFor="email">Email:</label>
                    <input type="email" name="email" value={email} onChange={(e) => setEmail(e.target.value)} />
                </div>
                <div>
                    <label htmlFor="password">Password:</label>
                    <input type="password" name="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                </div>
                <button type="submit">Register</button>
            </form>
        </div>
    );
};

export default Register;
