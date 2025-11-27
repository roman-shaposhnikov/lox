test:
	cargo test $(filter-out $@,$(MAKECMDGOALS))

run:
	cargo run $(filter-out $@,$(MAKECMDGOALS))

# Dummy target to consume all extra arguments
%:
	@:
