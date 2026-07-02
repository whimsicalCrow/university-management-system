window.thesisAuth = {
    async login(email, password) {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        });

        if (!response.ok) {
            return {
                success: false,
                role: null,
                message: 'Invalid email or password'
            };
        }

        const payload = await response.json();
        return {
            success: payload.success === true,
            role: payload.role ?? null,
            message: payload.message ?? null
        };
    }
};
