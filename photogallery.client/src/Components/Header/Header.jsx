import { Link, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { logout } from '../../userSlice';
import './Header.css';

const Header = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const user = useSelector((state) => state.user);

    const handleLogout = () => {
        dispatch(logout());
        navigate('/');
    };

    return (
        <header className="header">
            <Link to="/">
                <h1>Photo Gallery</h1>
            </Link>
            {!user.isAuthenticated ? (
                <div>
                    <Link to="/login">
                        <button>Login</button>
                    </Link>
                    <Link to="/register">
                        <button>Register</button>
                    </Link>
                </div>
                ) : (
                <div>
                    <button onClick={handleLogout}>Logout</button>
                        <Link to={"/" + user.userName}>
                        <button>My Albums</button>
                    </Link>
                </div>
            )}
        </header>
    );
};

export default Header;