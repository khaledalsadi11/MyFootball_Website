document.querySelectorAll('[data-password-target]').forEach(button => {
    button.addEventListener('click', () => {
        const input = document.getElementById(button.dataset.passwordTarget);
        if (!input) return;
        const show = input.type === 'password';
        input.type = show ? 'text' : 'password';
        button.setAttribute('aria-pressed', String(show));
        const label = button.dataset.passwordTarget === 'Input_ConfirmPassword' ? 'confirm password' : 'password';
        button.setAttribute('aria-label', `${show ? 'Hide' : 'Show'} ${label}`);
        button.querySelector('i').className = show ? 'bi bi-eye-slash' : 'bi bi-eye';
    });
});
