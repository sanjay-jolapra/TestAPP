document.addEventListener('DOMContentLoaded', function () {
    // Sidebar toggle
    const toggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const wrapper = document.getElementById('page-content-wrapper');

    if (toggle && sidebar) {
        toggle.addEventListener('click', function () {
            sidebar.classList.toggle('collapsed');
            if (window.innerWidth > 768) {
                wrapper.style.marginLeft = sidebar.classList.contains('collapsed') ? '0' : '260px';
            } else {
                sidebar.classList.toggle('show');
            }
        });
    }

    // Auto-dismiss alerts
    setTimeout(function () {
        document.querySelectorAll('.alert').forEach(function (el) {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(el);
            if (bsAlert) bsAlert.close();
        });
    }, 4000);

    // Init DataTables
    if (typeof $.fn.DataTable !== 'undefined') {
        $('.data-table').DataTable({
            pageLength: 25,
            responsive: true,
            language: { search: '', searchPlaceholder: 'Search...' }
        });
    }
});
