(function () {
    const config = window.accountantHomeConfig;
    const queueSection = document.getElementById('accountantQueueSection');
    const modalEl = document.getElementById('requestDetailsModal');

    if (!config || !queueSection || !modalEl) {
        return;
    }

    const customType = config.customType;
    const typeLabels = config.typeLabels;
    const statusLabels = config.statusLabels;
    const statusBadgeClasses = config.statusBadgeClasses;

    const detailLoading = document.getElementById('detailLoading');
    const detailError = document.getElementById('detailError');
    const detailContent = document.getElementById('detailContent');
    const detailRequestId = document.getElementById('detailRequestId');
    const detailEmployee = document.getElementById('detailEmployee');
    const detailType = document.getElementById('detailType');
    const detailCopies = document.getElementById('detailCopies');
    const detailReason = document.getElementById('detailReason');
    const detailStatus = document.getElementById('detailStatus');

    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

    function formatType(details) {
        const name = typeLabels[details.type] || details.type;
        if (details.type === customType && details.customTypeName) {
            return name + ' («' + details.customTypeName + '»)';
        }
        return name;
    }

    function formatEmployee(details) {
        if (details.employeeDepartment) {
            return details.employeeFullName + ' (' + details.employeeDepartment + ')';
        }
        return details.employeeFullName;
    }

    function resetDetailView() {
        detailLoading.classList.remove('d-none');
        detailError.classList.add('d-none');
        detailError.textContent = '';
        detailContent.classList.add('d-none');
    }

    function showDetailError(message) {
        detailLoading.classList.add('d-none');
        detailContent.classList.add('d-none');
        detailError.textContent = message;
        detailError.classList.remove('d-none');
    }

    function showDetails(details) {
        detailLoading.classList.add('d-none');
        detailError.classList.add('d-none');

        detailRequestId.textContent = String(details.id);
        detailEmployee.textContent = formatEmployee(details);
        detailType.textContent = formatType(details);
        detailCopies.textContent = String(details.copiesCount);
        detailReason.textContent = details.reason;

        const status = details.status;
        const badge = statusBadgeClasses[status] || 'text-bg-secondary';
        const statusName = statusLabels[status] || status;
        detailStatus.innerHTML = '<span class="badge ' + badge + '">' + statusName + '</span>';

        detailContent.classList.remove('d-none');
    }

    async function openRequestDetails(requestId) {
        resetDetailView();
        modal.show();

        try {
            const res = await fetch('/api/requests/' + requestId);
            const payload = await res.json().catch(function () { return {}; });

            if (!res.ok) {
                throw new Error(payload.error || 'Не удалось загрузить заявку.');
            }

            showDetails(payload);
        } catch (err) {
            showDetailError(err.message || 'Произошла ошибка.');
        }
    }

    queueSection.addEventListener('click', function (e) {
        const row = e.target.closest('.js-request-row');
        if (!row) return;

        const requestId = row.dataset.requestId;
        if (!requestId) return;

        openRequestDetails(requestId);
    });

    modalEl.addEventListener('hidden.bs.modal', resetDetailView);
})();
