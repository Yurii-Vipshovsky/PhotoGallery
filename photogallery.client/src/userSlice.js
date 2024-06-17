import { createSlice } from '@reduxjs/toolkit';

const initialState = {
    isAuthenticated: false,
    isAdmin: false,
    userName: null,
    token: null
};

const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {
        login: (state, action) => {
            state.isAuthenticated = true;
            state.isAdmin = action.payload.isAdmin;
            state.userName = action.payload.userName;
            state.token = action.payload.token;
        },
        logout: (state) => {
            state.isAuthenticated = false;
            state.isAdmin = false;
            state.userName = null;
            state.token = null;
        }
    }
});

export const { login, logout } = userSlice.actions;

export default userSlice.reducer;
