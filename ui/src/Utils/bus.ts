import { uuidGen } from "./uuidv4";

export type ContextCallback = (args: any) => void;

interface ListenerContext {
  publisher: string;
  id: string;
  callback: ContextCallback;
}

interface PublisherContext {
  name: string;
  callback: ContextCallback;
}

class SimpleEventBus {
  private publishers: PublisherContext[] = [];
  private listeners: ListenerContext[] = [];

  constructor() {}

  registerPublisher(name: string): ContextCallback {
    const checkPublisher = this.publishers.find((p) => p.name == name);

    if (checkPublisher) {
      return checkPublisher.callback;
    }

    const callback: ContextCallback = (args: any) => {
      for (let listener of this.listeners) {
        if (listener.publisher == name) {
          listener.callback(args);
        }
      }
    };

    this.publishers.push({ name, callback });
    return callback;
  }

  unRegisterPublisher(name: string): void {
    const idx = this.publishers.findIndex((p) => p.name == name);

    if (idx < 0) {
      console.error(
        "[R34Man][Error] Cannot unregister a not exiting publisher: %s",
        name
      );

      throw new Error("unregister without a exiting publisher");
    }
    this.publishers.splice(idx, 1);

    const toDelete: boolean[] = new Array(this.listeners.length).fill(false);

    for (let i = 0; i < this.listeners.length; ++i) {
      if (this.listeners[i].publisher == name) {
        toDelete[i] = true;
      }
    }
    this.listeners = this.listeners.filter((_, i) => !toDelete[i]);
  }

  listenTo(publisher: string, callback: ContextCallback): string {
    if (typeof callback != "function") {
      console.error("[R34Man][Error] callback of listeners must be callable");

      throw new Error("callback of listeners must be callable");
    }

    const listenerId = uuidGen();

    this.listeners.push({ publisher, callback, id: listenerId });

    return listenerId;
  }

  unListenTo(
    listenerId: string,
    onUnListenTo: CallableFunction = () => {}
  ): void {
    const idx = this.listeners.findIndex((l) => l.id == listenerId);

    if (idx < 0) {
      console.error("[R34Man][Error] Listener id %s not found", listenerId);

      throw new Error("listener not found");
    }

    this.listeners.splice(idx, 1);

    if (typeof onUnListenTo == "function") {
      onUnListenTo();
    }
  }
}

const bus = new SimpleEventBus();

export { bus };
