const App = {
    setup() {
        const state = Vue.reactive({
            email: '',
            password: '',
            isSubmitting: false,
            errors: {
                email: '',
                password: ''
            }
        });

        const validateForm = () => {
            state.errors.email = '';
            state.errors.password = '';
            let isValid = true;

            if (!state.email) {
                state.errors.email = 'Az email cím megadása kötelező.';
                isValid = false;
            } else if (!/\S+@\S+\.\S+/.test(state.email)) {
                state.errors.email = 'Kérjük, adjon meg egy érvényes email címet.';
                isValid = false;
            }

            if (!state.password) {
                state.errors.password = 'A jelszó megadása kötelező.';
                isValid = false;
            } else if (state.password.length < 6) {
                state.errors.password = 'A jelszónak legalább 6 karakterből kell állnia.';
                isValid = false;
            }

            return isValid;
        };

        const handleSubmit = async () => {
            try {
                state.isSubmitting = true;
                await new Promise(resolve => setTimeout(resolve, 300));

                if (!validateForm()) {
                    return;
                }

                const response = await AxiosManager.post('/Security/Login', {
                    email: state.email,
                    password: state.password
                });

                if (response.data.code === 200) {
                    StorageManager.saveLoginResult(response.data);
                    Swal.fire({
                        icon: 'success',
                        title: 'Sikeres bejelentkezés',
                        text: 'Tovább a fő oldalra...',
                        timer: 2000,
                        showConfirmButton: false
                    });

                    setTimeout(() => {
                        window.location.href = '/Profiles/MyProfile';
                    }, 2000);
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Sikertelen bejelentkezés',
                        text: response.data.message || 'Kérjük, ellenőrizze az adatait.',
                        confirmButtonText: 'Újra próbálom'
                    });
                }
            } catch (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Hiba történt',
                    text: error.response?.data?.message || 'Próbáld újra.',
                    confirmButtonText: 'OK'
                });
            } finally {
                state.isSubmitting = false;
            }
        };

        return {
            state,
            handleSubmit
        };
    }
};

Vue.createApp(App).mount('#app');