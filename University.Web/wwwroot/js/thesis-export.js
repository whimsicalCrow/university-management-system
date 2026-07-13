window.exportThesisForm = function (data) {
    var workTypeItems = [
        'Σχεδιασμό και ανάπτυξη συστήματος',
        'Συγκριτική επισκόπηση ή μελέτη, και πλαίσιο αξιολόγησης',
        'Ανάλυση και σχεδιασμό μοντέλων',
        'Θεωρητική μελέτη, ανάπτυξη ή ανάλυση πλατφόρμας ή αλγορίθμων',
        'Πρότυπη κατασκευή'
    ];

    var selectedTypes = data.workTypes || [];

    var objectivesHtml = '';
    if (data.objectives && data.objectives.trim()) {
        var lines = data.objectives.split('\n').map(function (l) { return l.trim(); }).filter(function (l) { return l.length > 0; });
        objectivesHtml = lines.map(function (l) { return '<li>' + escapeHtml(l) + '</li>'; }).join('');
    }

    var workTypesHtml = workTypeItems.map(function (item) {
        var checked = selectedTypes.indexOf(item) !== -1;
        var box = checked ? '&#9745;' : '&#9744;';
        return '<tr><td style="padding:4px 8px; font-size:13px;">' + box + ' ' + escapeHtml(item) + '</td></tr>';
    }).join('');

    var html = '<!DOCTYPE html><html lang="el"><head>' +
        '<meta charset="UTF-8"/>' +
        '<title>Περιγραφή Θέματος Διπλωματικής</title>' +
        '<style>' +
        'body { font-family: Arial, sans-serif; margin: 30px; color: #000; font-size: 13px; }' +
        'h1 { text-align: center; font-size: 14px; font-weight: bold; margin-bottom: 16px; text-transform: uppercase; }' +
        'table { width: 100%; border-collapse: collapse; }' +
        'td, th { border: 1px solid #000; padding: 6px 10px; vertical-align: top; }' +
        '.label { font-weight: bold; white-space: nowrap; }' +
        '.section-title { font-weight: bold; background: #f0f0f0; padding: 4px 10px; border: 1px solid #000; }' +
        'ul { margin: 4px 0; padding-left: 20px; }' +
        '@media print { body { margin: 10mm; } }' +
        '</style></head><body>' +
        '<h1>Περιγραφή Προτεινόμενου Θέματος Διπλωματικής Εργασίας</h1>' +
        '<table>' +
        '<tr><td class="label" style="width:80px;">Τίτλος:</td><td colspan="3">' + escapeHtml(data.title || '') + '</td></tr>' +
        '<tr>' +
        '  <td class="label">Επιβλέπων:</td><td>' + escapeHtml(data.supervisingProfessor || '') + '</td>' +
        '  <td class="label" style="width:60px;">e-mail:</td><td>' + escapeHtml(data.supervisorEmail || '') + '</td>' +
        '</tr>' +
        '<tr>' +
        '  <td class="label">Άτομα:</td><td colspan="3">' + escapeHtml(String(data.maxStudents || '1')) + '</td>' +
        '</tr>' +
        '<tr><td class="label">Στόχοι</td><td colspan="3"><ul>' + objectivesHtml + '</ul></td></tr>' +
        '<tr><td class="label">Αντικείμενο:</td><td colspan="3">' + escapeHtml(data.description || '') + '</td></tr>' +
        '<tr><td colspan="4" class="section-title">Η εργασία περιλαμβάνει</td></tr>' +
        workTypesHtml +
        '<tr><td colspan="4" class="section-title">Σχετιζόμενα Μαθήματα</td></tr>' +
        '<tr><td class="label">Πρωτεύοντα:</td><td colspan="3">' + escapeHtml(data.relatedCoursesPrimary || '') + '</td></tr>' +
        '<tr><td class="label">Δευτερεύοντα:</td><td colspan="3">' + escapeHtml(data.relatedCoursesSecondary || '') + '</td></tr>' +
        '<tr><td class="label">Υποχρεώσεις Παρουσίας:</td><td colspan="3">' + escapeHtml(data.attendanceObligations || '') + '</td></tr>' +
        '</table>' +
        '<script>window.onload = function(){ window.print(); };<\/script>' +
        '</body></html>';

    var win = window.open('', '_blank', 'width=800,height=900');
    if (win) {
        win.document.open();
        win.document.write(html);
        win.document.close();
    }
};

function escapeHtml(text) {
    return String(text)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}
