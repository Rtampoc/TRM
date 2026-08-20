// Tooling Request Form (TRF) JavaScript Interactions

document.addEventListener('DOMContentLoaded', function () {
    initializeTRFForm();
});

function initializeTRFForm() {
    // Initialize line item management
    initializeLineItems();

    // Initialize form action buttons
    initializeFormActions();

    // Initialize tab switching
    initializeTabSwitching();

    // Initialize line item row number updates
    updateLineItemRowNumbers();
}

/**
 * Initialize line item management (add/remove rows, calculations)
 */
function initializeLineItems() {
    // Add row button click handlers
    document.querySelectorAll('button[data-table]').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const tableId = this.dataset.table;
            addLineItemRow(tableId);
        });
    });

    // Remove row button click handlers (use event delegation for dynamic rows)
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('btn-remove-row')) {
            e.preventDefault();
            const row = e.target.closest('tr');
            const table = row.closest('table');

            if (table.querySelectorAll('tbody tr').length > 1) {
                row.remove();
                updateLineItemRowNumbers();
            } else {
                alert('You must keep at least one row in the table.');
            }
        }
    });

    // Add calculation handlers for cost fields
    document.addEventListener('change', function (e) {
        if (e.target.closest('.line-item-row')) {
            const row = e.target.closest('tr');
            calculateLineItemTotal(row);
        }
    });

    document.addEventListener('input', function (e) {
        if (e.target.closest('.line-item-row') && 
            (e.target.type === 'number' || e.target.type === 'text')) {
            const row = e.target.closest('tr');
            if (row) {
                calculateLineItemTotal(row);
            }
        }
    });
}

/**
 * Add a new line item row to the specified table
 */
function addLineItemRow(tableId) {
    const table = document.getElementById(tableId);
    const tbody = table.querySelector('tbody');
    const lastRow = tbody.querySelector('tr:last-child');
    const newRow = lastRow.cloneNode(true);

    // Clear input values in the new row
    newRow.querySelectorAll('input').forEach(input => {
        input.value = '';
    });

    // Update row number
    const rowNum = tbody.querySelectorAll('tr').length + 1;
    newRow.querySelector('td:first-child').textContent = rowNum;

    // Reset readonly fields (Total Cost)
    const totalCostInput = newRow.querySelector('input[readonly]');
    if (totalCostInput) {
        totalCostInput.value = '';
    }

    tbody.appendChild(newRow);
    newRow.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

/**
 * Calculate total cost for a line item row
 */
function calculateLineItemTotal(row) {
    const inputs = row.querySelectorAll('input[type="number"]');

    if (inputs.length >= 7) {
        // Assuming columns: Length, Width, Height, TotalKgs, EstMachine, Material, Machining, Testing, Other, Total
        const estMachine = parseFloat(inputs[4]?.value) || 0;
        const material = parseFloat(inputs[5]?.value) || 0;
        const machining = parseFloat(inputs[6]?.value) || 0;
        const testing = parseFloat(inputs[7]?.value) || 0;
        const other = parseFloat(inputs[8]?.value) || 0;

        const total = estMachine + material + machining + testing + other;

        // Set the readonly total field
        const totalField = row.querySelector('input[readonly]');
        if (totalField) {
            totalField.value = total.toFixed(2);
        }
    }
}

/**
 * Update line item row numbers
 */
function updateLineItemRowNumbers() {
    document.querySelectorAll('.line-items-table tbody').forEach(tbody => {
        tbody.querySelectorAll('tr').forEach((row, index) => {
            row.querySelector('td:first-child').textContent = index + 1;
        });
    });
}

/**
 * Initialize form action buttons (Submit, Save Draft, Print)
 */
function initializeFormActions() {
    const form = document.getElementById('trfForm');
    const formStatusInput = document.getElementById('formStatus');

    // Print button
    document.querySelectorAll('#btnPrint, #btnPrintBottom').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            window.print();
        });
    });

    // Save Draft button
    document.querySelectorAll('#btnSaveDraft, #btnSaveDraftBottom').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            formStatusInput.value = 'Draft';
            saveFormData(form, 'Draft');
        });
    });

    // Submit button
    document.querySelectorAll('#btnSubmit, #btnSubmitBottom').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            if (form.checkValidity() === false) {
                e.stopPropagation();
                form.classList.add('was-validated');
                alert('Please fill in all required fields.');
                return;
            }
            formStatusInput.value = 'Submitted';
            form.submit();
        });
    });

    // Cancel button
    document.querySelectorAll('#btnCancel').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            if (confirm('Are you sure you want to cancel? Any unsaved changes will be lost.')) {
                form.reset();
                window.history.back();
            }
        });
    });
}

/**
 * Save form data (Draft or Submit)
 */
function saveFormData(form, status) {
    const formData = new FormData(form);
    formData.set('FormStatus', status);

    // Collect line items from both tables
    const lineItems = collectLineItems();
    formData.set('LineItems', JSON.stringify(lineItems));

    // Send AJAX request
    fetch(form.action || window.location.href, {
        method: 'POST',
        body: formData,
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
    .then(response => {
        if (response.ok) {
            alert(`Form ${status === 'Draft' ? 'saved as draft' : 'submitted'} successfully!`);
            if (status === 'Submitted') {
                form.reset();
                window.location.href = '/index'; // Redirect to dashboard
            }
        } else {
            alert('Error saving form. Please try again.');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('An error occurred while saving the form.');
    });
}

/**
 * Collect line items from both CNC and Knife tables
 */
function collectLineItems() {
    const lineItems = [];

    document.querySelectorAll('.line-items-table tbody tr').forEach((row, index) => {
        const cells = row.querySelectorAll('td');
        if (cells.length > 0) {
            const inputs = row.querySelectorAll('input');
            const toolType = row.dataset.toolType || 'CNC';

            const item = {
                lineNumber: index + 1,
                joNumber: inputs[0]?.value || '',
                toolDescriptor: inputs[1]?.value || '',
                length: inputs[2]?.value || null,
                width: inputs[3]?.value || null,
                height: inputs[4]?.value || null,
                totalKgs: inputs[5]?.value || null,
                estMachineCost: inputs[6]?.value || null,
                materialCost: inputs[7]?.value || null,
                machiningCostPHP: inputs[8]?.value || null,
                testingCost: inputs[9]?.value || null,
                otherCost: inputs[10]?.value || null,
                totalCostPHP: inputs[11]?.value || null,
                mouldSelling: inputs[12]?.value || '',
                gpRate: inputs[13]?.value || null,
                remarks: inputs[14]?.value || '',
                toolType: toolType
            };

            if (Object.values(item).some(val => val !== '' && val !== null)) {
                lineItems.push(item);
            }
        }
    });

    return lineItems;
}

/**
 * Initialize tab switching
 */
function initializeTabSwitching() {
    const tabButtons = document.querySelectorAll('#toolTypeTabs .nav-link');

    tabButtons.forEach(btn => {
        btn.addEventListener('shown.bs.tab', function () {
            updateLineItemRowNumbers();
        });
    });
}

/**
 * Export form data to JSON (for debugging or data transfer)
 */
function exportFormData() {
    const form = document.getElementById('trfForm');
    const formData = new FormData(form);
    const data = Object.fromEntries(formData);
    data.lineItems = collectLineItems();

    console.log('Form Data:', data);
    return data;
}

/**
 * Validate form before submission
 */
function validateTRFForm() {
    const trfNo = document.getElementById('trfNo').value.trim();
    const customerId = document.getElementById('customerId').value;
    const model = document.getElementById('model').value.trim();

    if (!trfNo) {
        alert('TRF No. is required.');
        return false;
    }

    if (!customerId) {
        alert('Please select a Customer.');
        return false;
    }

    if (!model) {
        alert('Model name is required.');
        return false;
    }

    // Check if at least one line item has data
    const hasLineItems = document.querySelectorAll('.line-items-table tbody tr').some(row => {
        const inputs = row.querySelectorAll('input');
        return Array.from(inputs).some(input => input.value.trim() !== '');
    });

    if (!hasLineItems) {
        alert('Please add at least one line item.');
        return false;
    }

    return true;
}

/**
 * Reset form to initial state
 */
function resetTRFForm() {
    const form = document.getElementById('trfForm');
    form.reset();

    // Reset line items tables to single row
    document.querySelectorAll('.line-items-table tbody').forEach(tbody => {
        const rows = tbody.querySelectorAll('tr');
        for (let i = rows.length - 1; i > 0; i--) {
            rows[i].remove();
        }
        rows[0].querySelectorAll('input').forEach(input => {
            input.value = '';
        });
    });

    updateLineItemRowNumbers();
    document.getElementById('formStatus').value = 'Draft';
}
