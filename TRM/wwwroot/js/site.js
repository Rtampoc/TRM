// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minification

// User Management Module
const UserManagement = {
    init() {
        this.setupModal();
        this.setupSearch();
        this.setupFilters();
        this.setupTableActions();
    },

    setupModal() {
        const modal = document.getElementById('addUserModal');
        if (!modal) return;

        // Reset form when modal is closed
        modal.addEventListener('hidden.bs.modal', () => {
            this.resetForm();
        });

        // Setup role selection
        document.querySelectorAll('.role-option input').forEach(input => {
            input.addEventListener('change', (e) => {
                // Highlight selected role
                document.querySelectorAll('.role-option-label').forEach(label => {
                    label.style.borderColor = '';
                });
                if (e.target.checked) {
                    const label = e.target.nextElementSibling;
                    if (label) {
                        label.style.borderColor = 'var(--primary)';
                    }
                }
            });
        });
    },

    setupSearch() {
        const searchInput = document.getElementById('searchInput');
        if (!searchInput) return;

        let debounceTimer;
        searchInput.addEventListener('input', () => {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                // Could implement live search via AJAX here
                console.log('Searching for:', searchInput.value);
            }, 300);
        });
    },

    setupFilters() {
        const roleFilter = document.getElementById('roleFilter');
        const statusFilter = document.getElementById('statusFilter');

        if (roleFilter) {
            roleFilter.addEventListener('change', () => {
                this.applyFilters();
            });
        }

        if (statusFilter) {
            statusFilter.addEventListener('change', () => {
                this.applyFilters();
            });
        }
    },

    setupTableActions() {
        // Setup delete button confirmations
        document.querySelectorAll('.delete-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                if (!confirm('Are you sure you want to delete this user?')) {
                    e.preventDefault();
                }
            });
        });

        // Setup toggle status confirmations
        document.querySelectorAll('.toggle-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const isActive = btn.closest('.user-table-row')?.querySelector('.status-badge.active');
                const message = isActive
                    ? 'Are you sure you want to deactivate this user?'
                    : 'Are you sure you want to activate this user?';

                if (!confirm(message)) {
                    e.preventDefault();
                }
            });
        });
    },

    applyFilters() {
        // Implementation for filtering via AJAX or form submission
        const form = document.querySelector('form');
        if (form && form.method === 'post') {
            // Could submit via AJAX or regular form
            // form.submit();
        }
    },

    resetForm() {
        const form = document.querySelector('#addUserModal form');
        if (form) {
            form.reset();
            // Clear radio button selections
            document.querySelectorAll('.role-option input').forEach(input => {
                input.checked = false;
            });
        }
    }
};

// Initialize when document is ready
document.addEventListener('DOMContentLoaded', () => {
    UserManagement.init();
});
