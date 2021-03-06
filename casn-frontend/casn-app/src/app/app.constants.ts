/* Define app-level constants which can be imported to any module */
export class Constants {
    public readonly MENU_ITEMS = [
      {
        name: "Home",
        address: "/",
        icon: "dashboard",
        dispatcherOnly: false
      },
      {
        name: "My Drives",
        address: "/my-drives",
        icon: "local_taxi",
        dispatcherOnly: false
      },
      {
        name: "My Achievements",
        address: "/stats",
        icon: "grade",
        dispatcherOnly: false
      },
      {
        name: "Schedule a Ride",
        address: "/caller",
        icon: "departure_board",
        dispatcherOnly: true
      },
      {
        name: "View Schedule",
        address: "/view-schedule",
        icon: "event_note",
        dispatcherOnly: false
      },
      // {
      //   name: "Message Volunteers",
      //   address: "/message",
      //   icon: "chat",
      //   dispatcherOnly: true
      // },
    ];

  // These routes are only accessible to dispatchers.
  public readonly RESTRICTED_ROUTES = [
    '/caller',
    '/message',
    '/appointment'
  ];

  public readonly SERVICE_PROVIDER_MAP_MARKERS = {
    1: 'assets/img/marker_clinic.png',
    2: 'assets/img/marker_court.png',
    3: 'assets/img/marker_lodging.png',
    4: 'assets/img/marker_airport.png',
    default: 'assets/img/marker_cluster.png'
  };
}
