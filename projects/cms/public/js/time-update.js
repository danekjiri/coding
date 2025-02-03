(function() {
    const MS_PER_DAY = 86400000;
    const DAYS_OF_WEEK = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

    initTimeUpdates();

    // ---
    // TIME UPDATE

    function initTimeUpdates() {
        updateTimes();
        setInterval(updateTimes, 1000);
    }
  
    function updateTimes() {
        const timeCells = document.querySelectorAll('.timeUpdate');
        timeCells.forEach(cell => {
            const dateTime = parseMySQLDateTime(cell.getAttribute('data-time'));
            cell.textContent = formatRelativeTime(dateTime);
        });
        }

        function parseMySQLDateTime(dateTime) {
        const standardized = dateTime.replace(' ', 'T');
        return new Date(standardized);
        }

        function formatRelativeTime(givenDateTime) {
        const now = new Date();
        const diffMs = now - givenDateTime;
        const diffSec = Math.floor(diffMs / 1000);
        if (diffSec < 0) {
            return 'error (future time)';
        } else if (diffSec < 60) {
            return diffSec + ' seconds ago';
        }

        const diffMin = Math.floor(diffSec / 60);
        if (diffMin < 60) {
            return diffMin + ' minutes ago';
        }

        const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        const givenDay = new Date(givenDateTime.getFullYear(), givenDateTime.getMonth(), givenDateTime.getDate());
        const dayDiff = Math.floor((today - givenDay) / MS_PER_DAY); // ms in one day
        if (dayDiff === 0) {
            return "today at " + formatHHmm(givenDateTime);
        } else if (dayDiff === 1) {
            return "yesterday at " + formatHHmm(givenDateTime);
        } else if (dayDiff < 7) {
            const weekday = DAYS_OF_WEEK[dateVal.getDay()];
            return weekday + " at " + formatHHmm(givenDateTime);
        }
        
        return givenDateTime.toTimeString();
    }

    function formatHHmm(givenDateTime) {
        const hh = String(givenDateTime.getHours()).padStart(2, '0');
        const mm = String(givenDateTime.getMinutes()).padStart(2, '0');
        return hh + ":" + mm;
    }
})();