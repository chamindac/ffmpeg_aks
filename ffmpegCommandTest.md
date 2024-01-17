### Get Metadata
ffprobe -v quiet -show_format -show_streams -print_format json 227ac968-7f98-41b5-806c-cd966f41128c/227ac968-7f98-41b5-806c-cd966f41128c

ffprobe -v quiet -skip_frame nokey -select_streams v:0 -show_entries frame=pkt_pts_time -print_format json 227ac968-7f98-41b5-806c-cd966f41128c/227ac968-7f98-41b5-806c-cd966f41128c