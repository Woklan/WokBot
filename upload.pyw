from win10toast import ToastNotifier
import subprocess

toast = ToastNotifier()

toast.show_toast("Docker Push Started", "Docker Push Started", duration=5)
subprocess.call("docker buildx build --platform linux/arm64 -t woklan/wokbot --push .", shell=True)
toast.show_toast("Docker Push Completed", "Docker Push Completed", duration=20)
