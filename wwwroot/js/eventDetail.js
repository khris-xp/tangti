function getOrganiserById(endpointUrl) {
  fetch(endpointUrl)
    .then((response) => {
      if (response.ok) {
        return response.json();
      }
      throw new Error('Error');
    })
    .then((data) => {
      document.getElementById('organiser').innerHTML =
        data.firstName + ' ' + data.lastName;
    })
    .catch((error) => {
      console.error(error);
    });
}

function updateEnrollment(endpointUrl) {
  const eventId = '@Model.Event.Id';
  const userId = localStorage.getItem('id');

  fetch(endpointUrl, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      eventId: eventId,
      userId: userId,
    }),
  })
    .then((response) => {
      if (response.ok) {
        return response.json();
      }
      throw new Error('Error');
    })
    .catch((error) => {
      console.error(error);
    });
}

function enrollEvent() {
  const endpointUrl = 'https://kmitltangti.azurewebsites.net/api/enroll/update';
  updateEnrollment(endpointUrl);
}

function unenrollEvent() {
  const endpointUrl =
    'https://kmitltangti.azurewebsites.net/api/enroll/unenroll';
  updateEnrollment(endpointUrl);
}

function formatDate(date) {
  const parts = date.split(' ');
  const dateString = parts[0];
  const timeString = parts[1] + ' ' + parts[2];

  const dateParts = dateString.split('/');
  const month = parseInt(dateParts[0]) - 1;
  const day = parseInt(dateParts[1]);
  const year = parseInt(dateParts[2]);

  let timeParts = timeString.split(':');
  let hours = parseInt(timeParts[0]);
  const minutes = parseInt(timeParts[1]);

  const d = new Date(year, month, day, hours, minutes);

  const options = { year: 'numeric', month: 'short', day: '2-digit' };
  return d.toLocaleDateString('en', options);
}
